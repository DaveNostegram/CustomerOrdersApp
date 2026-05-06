using CustomerOrdersApp.Contracts;
using CustomerOrdersApp.Domain.Customers;
using CustomerOrdersApp.Domain.Orders;

namespace CustomerOrdersApp.Application.Interfaces.Repositories;

public interface ICustomerRepo
{
    Task StageCustomers(List<Customer> customers, CancellationToken ct);
    Task StageOrders(List<Order> orders, CancellationToken ct);
    Task StageOrderItems(List<OrderItem> orderItems, CancellationToken ct);
    Task<bool> ExistsByPublicIdAsync(int publicId, CancellationToken ct);
    Task ClearAllData(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<Dictionary<int, int>> GetCustomerIdsByPublicIdsAsync(IReadOnlyCollection<int> publicIds, CancellationToken ct);
    Task<Dictionary<int, int>> GetOrderIdsByPublicIdsAsync(IReadOnlyCollection<int> publicIds, CancellationToken ct);
    Task<List<Customer>> GetAllCustomers(CancellationToken ct);
    Task<List<CustomerDto>> GetAllCustomerDtos(CancellationToken ct);

}