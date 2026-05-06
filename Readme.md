setup:
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

Without additional guidance I use clean architecture for the project layout. To developers unfamiliar with this layout the one day scope could be at risk. I work day to day with this layout so did not see any disadvantages. Had I had access to the existing system I would have used this in my consideration, should migrations or merging be required. 

## Setup domain entities. 

All inherit Base entity, giving Id and PublicId. PublicId is a int to match the excel documents. Ideally this would be a Guid.

State should be an enum minimum due to using in the discount as having as a string has risk with future input, plus state is a limited so not necessary to be a separate table. 

Order had order status which I set as Enum as won't have any meaningful growth. Customer Id is a foreign key

Order item has no publicId, as such I generated one. "Item_Id" I processed but did nothing with. My assumption is there would be a Items table that I would process, but with time contrains I did not create this entity. Added FinalPrice to OrderItem so I could process Discounts. For costing you would not want to calculate this in memory.

## Backend

As general rule I only save changes outside of repos rather than as part of the repo. Currently it's a separate call. In future move to unit of work for saving changes from Repo, as data gets more complex as we save across multiple repos it'll become an issue.


## CSVs

### Decisions

I made an assumption on Order Status, based on shipped date, and named the Enum accordingly.
I only process CSV, handling Excel would not fit within the timeframe
I created a object to parse the files into before validating. In a real world scenario and larger files you'd likely put these into a table and handle them elsewhere so the user is not waiting for completion, then send a message once done.

### Future refactor
ImportCustomersResult errors ideally should carry more than just string. Within the timeframe I simplified this.
Added that Validate Headers also checks for duplicates but isn't clear from method name. Should refactor ideally
Each uploader has repeatative code, this code be refactored but for timeframe was copy pasted.
I do not currently validate correctly for already exists in the system due to time contrains. This would need to exist in future.
Headers of files are currently hard coded. It's worthwhile having validation checking for alternatives i.e. "first_name" could also accept "firstname" or "first name"
Future concerns for mapping State, if users send files with full state name rather than code.
Mapping is currently within each cmd. Should ideally move elsewhere, should it be used again.




## Discount

With discount I took onboard the 

Currently the discount repo is hard coded, it can be wired to add more later.

It allows flexibility of new discounts. It handles the possibility of multiple discounts and picks the highest.