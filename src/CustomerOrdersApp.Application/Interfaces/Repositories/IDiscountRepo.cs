using CustomerOrdersApp.Domain.Customers;
using CustomerOrdersApp.Domain.Discounts;

namespace CustomerOrdersApp.Application.Interfaces.Repositories;

public interface IDiscountRepo
{
    Task<List<Discount>> GetDiscounts(CancellationToken ct);
    Task ApplyDiscountToUnshippedOrders(Customer customer, decimal discountRate, CancellationToken ct);
    Task ApplyDiscountToAllOrders(Customer customer, decimal discountRate, CancellationToken ct);
}