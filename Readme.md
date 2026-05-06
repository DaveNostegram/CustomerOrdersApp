# Setup
docker compose up -d

dotnet ef database update `
  --project src\CustomerOrdersApp.Infrastructure `
  --startup-project src\CustomerOrdersApp.Api

cd C:\Code\CustomerOrdersApp\src\CustomerOrdersApp.Api
dotnet run

cd C:\Code\CustomerOrdersApp\frontend\CustomerOrdersApp.Web
npm run dev

Navigate to Upload Files. Upload each file.


# Decisions

- Clean Architecture structure
- CQRS with MediatR
- EF Core + SQL Server
- Vue frontend
- CSV ingestion pipeline
- Domain-event driven discount processing

---
## Setup domain entities. 

All inherit Base entity, giving Id and PublicId. PublicId is an int to match the source data. PublicId remains an int to align with the source data identifiers. In a production system I would likely separate external identifiers from internal GUID-based identities.

Customer:
- State data set is small, stable, and finite. As such I parsed as enum, as string would risk discount functionality.
- Phone is optional
- Email should be validated as an email

Order:
- Status data set is small, stable, and finite. As such I parsed as enum and by setting to an enum we enable better FE experience. 
- Customer Id is a foreign key
- Shipped date is optional, and appears to correlate to order_status 4. As such my assumption is that 4 is "Shipped"

OrderItem: 
- No publicId so I generated one. 
- "Item_Id" processed but did nothing with. My assumption is there would also be an Items table/entity in the wider domain model, however the supplied dataset did not require implementing it. 
- Added FinalPrice to OrderItem so I could process Discounts. I chose to not handle in memory so that calculating totals is easier as each item can have a different discount. This will later simplify Invoicing. 

---

## Layout
Without additional guidance I use clean architecture for the project layout. To developers unfamiliar with this layout the one day scope could be at risk. I work day to day with this layout so did not see any disadvantages. It would be beneficial to match the staff tech stack if growth of the system is expected. An existing system is mentioned in the spec, if migration or merging was required this should be reviewed before starting work. I made the assumption this would act like a microservice and thus have no coupling or requirements to the other system.

As a general rule I prefer persistence boundaries to exist outside repositories rather than calling `SaveChanges` directly within them. In larger systems I would typically expose a narrower commit abstraction to avoid application code depending directly on the DbContext and to better control transactional consistency across repositories.

---

## CSVs

### Decisions

I made an assumption on Order Status, based on shipped date, and named the Enum accordingly.
I only process CSV files.
I created an object to parse the files into before validating. In a real world scenario and larger files you'd likely put these into a table and handle them elsewhere so the user is not waiting for completion, then send a message once done.

### Future refactor
- ImportCustomersResult errors ideally should carry more than just string.
- Validate Headers checks for duplicates but isn't clear from method name. The method name should better reflect this behaviour.
- Each uploader currently contains repetitive logic that could be extracted into shared import infrastructure.
- Duplicate detection against existing persisted records should be added.
- Headers of files are currently hard coded. It's worthwhile having validation checking for alternatives i.e. "first_name" could also accept "firstname" or "first name"
- Future concerns for mapping State, if users send files with full state name rather than code.
- Mapping is currently within each cmd. Should ideally move elsewhere, should it be used again.

---

## Discount


With discount I took onboard the idea to not keep it tightly coupled and created a DiscountService. This handles all the logic and domain event. The command is only exposed to whether or not the customer has discount, discount amount and the reason so that we could return these for FE if wanted.

The discount pipeline was designed to support additional discount rules by evaluating all applicable discounts and selecting the largest valid discount for the customer. It currently *does not* check against the existing discount to confirm whether that's bigger or not. It needs to do this in future or we risk increasing prices and unhappy customers.

Discount also has a Type so in future another method of discount could be used e.g. Customer loyalty. You'll see ExampleFutureDiscount in a switch to preview this.

`src\CustomerOrdersApp.Infrastructure\Repositories\DiscountRepo.cs` Discount definitions are currently in-memory rather than persisted. If you want to test it you would only need to change the values here.

### Event 
I chose to use a MediatR notification for this as I already used MediatR so it felt like an easy transition. The current event handler logs to the console for demonstration purposes which you should see in your API window on apply discounts. Ideally this would actually post to an Audit table or similar. 

Without the concept of knowing how the event is used I chose to raise an event per customer. This could be used in different ways:
- Audit to validate discounts
- Send emails/notifications to customers as they would be pleased to hear they get a discount.
- Report on how often discounts are given.
- Trigger invoice, or correct invoice. 
- Confirm customer remains valid for the discount.

### Assumptions
I only currently apply discount to unshipped orders. My assumption is once an order is shipped it may be paid for. I still generated a version that applies regardless that can easily be swapped if necessary by changing `src\CustomerOrdersApp.Application\Discounts\DiscountService.cs` line 50 from `await _discountRepo.ApplyDiscountToUnshippedOrders(customer, bestDiscount.Amount, ct);` to `await _discountRepo.ApplyDiscountToAllOrders(customer, bestDiscount.Amount, ct);`.

---

## Tests
I created unit tests. My general concept is tests first then write code to complete them. I prioritised validating the higher-risk business logic with unit tests rather than pursuing broader test coverage. You will see these tests in `tests\CustomerOrdersApp.UnitTests`

I did not create any Integration tests as I did not do any complex data arrangement that I felt required it within the timeframe. For production these tests should exist. 

---

## Frontend
Front end is intentionally lightweight and accelerated with AI assistance to show a proof of concept so I could focus the available time on backend architecture and domain concerns. It uses Vue.js with all code existing in `frontend\CustomerOrdersApp.Web\src\App.vue`.

For user experience, customers auto reload when uploading, clearing all data, or applying discounts.

### Additional Improvements
Customers:
- Styled the tables better
- Removed the expand button when no orders
- Add paging (with a new GetPaged)
- Add filters.

Upload Files: 
- Clear data per upload
- Log of the last uploads. 

Discounts: 
- Ability to add additional discounts in an admin managed area.

### Future refactor
- Customers tab/Upload Files tab would move to separate components
- I would have a service .ts for each controller.
- I would move the types into separate .ts for each api
- Separate out the main.css

---