using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Application.Interfaces.Services;
using CustomerOrdersApp.Domain.Customers;
using CustomerOrdersApp.Domain.Enum;
using CustomerOrdersApp.Domain.Orders;
using MediatR;

namespace CustomerOrdersApp.Application.Discounts;

public class DiscountService : IDiscountService
{
    private readonly IDiscountRepo _discountRepo;
    private readonly IMediator _mediator;

    public DiscountService(IDiscountRepo discountRepo, IMediator mediator)
    {
        _discountRepo = discountRepo;
        _mediator = mediator;
    }
    public async Task<DiscountResult> ApplyDiscountAsync(Customer customer, CancellationToken ct)
    {
        var discounts = await _discountRepo.GetDiscounts(ct);
        List<DiscountResult> customerDiscounts = new();
        foreach (var discount in discounts)
        {
            var matched = discount.Type switch
            {
                DiscountTypeEnum.State =>
                    customer.State.ToString() == discount.Value,

                DiscountTypeEnum.ExampleFutureDiscount =>
                    customer.City.ToString() == discount.Value,

                _ => false
            };

            if (matched)
            {
                customerDiscounts.Add(new DiscountResult
                {
                    HasDiscount = true,
                    Amount = discount.DiscountAmount,
                    Reason = $"{discount.Type} matched discount rule"
                });
            }
        }
        if (customerDiscounts.Any())
        {
            var bestDiscount = customerDiscounts.OrderByDescending(e => e.Amount).First();
            await _discountRepo.ApplyDiscountToUnshippedOrders(customer, bestDiscount.Amount, ct);

            await _mediator.Publish(
        new CustomerDiscountApplied(
            CustomerId: customer.Id,
            DiscountAmount: bestDiscount.Amount,
            Reason: bestDiscount.Reason,
            AppliedAt: DateTime.UtcNow
        ),
        ct);

            return bestDiscount;
        }

        return new DiscountResult { HasDiscount = false };
    }
}