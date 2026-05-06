using CustomerOrdersApp.Application.Discounts;
using CustomerOrdersApp.Domain.Customers;

namespace CustomerOrdersApp.Application.Interfaces.Services;

public interface IDiscountService
{
    Task<DiscountResult> ApplyDiscountAsync(Customer customer, CancellationToken ct);
}