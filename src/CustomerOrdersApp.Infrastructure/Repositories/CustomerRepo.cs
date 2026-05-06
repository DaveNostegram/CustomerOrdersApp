using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Contracts;
using CustomerOrdersApp.Domain.Customers;
using CustomerOrdersApp.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrdersApp.Infrastructure.Repositories;

public class CustomerRepo(AppDbContext context) : ICustomerRepo
{
    private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task StageCustomers(List<Customer> customers, CancellationToken ct)
    {
        _context.AddRange(customers);
    }

    public async Task StageOrders(List<Order> orders, CancellationToken ct)
    {
        _context.AddRange(orders);
    }
    public async Task StageOrderItems(List<OrderItem> orderItems, CancellationToken ct)
    {
        _context.AddRange(orderItems);
    }

    public async Task ClearAllData(CancellationToken ct)
    {
        _context.Customers.RemoveRange(_context.Customers);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
    public Task<bool> ExistsByPublicIdAsync(int publicId, CancellationToken ct)
    {
        return _context.Customers.AnyAsync(x => x.PublicId == publicId, ct);
    }
    public async Task<Dictionary<int, int>> GetCustomerIdsByPublicIdsAsync(IReadOnlyCollection<int> publicIds, CancellationToken ct)
    {
        return await _context.Customers
            .Where(x => publicIds.Contains(x.PublicId))
            .ToDictionaryAsync(x => x.PublicId, x => x.Id, ct);
    }
    public async Task<Dictionary<int, int>> GetOrderIdsByPublicIdsAsync(IReadOnlyCollection<int> publicIds, CancellationToken ct)
    {
        return await _context.Orders
            .Where(x => publicIds.Contains(x.PublicId))
            .ToDictionaryAsync(x => x.PublicId, x => x.Id, ct);
    }
    public async Task<List<Customer>> GetAllCustomers(CancellationToken ct)
    {
        return await _context.Customers.ToListAsync(ct);
    }

    public async Task<List<CustomerDto>> GetAllCustomerDtos(CancellationToken ct)
    {
        return await _context.Customers
            .AsNoTracking()
            .Select(customer => new CustomerDto
            {
                Id = customer.Id,

                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,

                Street = customer.Street,
                City = customer.City,
                State = customer.State.ToString(),
                ZipCode = customer.ZipCode,

                PhoneNumber = customer.PhoneNumber,

                TotalSpend = customer.Orders
                    .SelectMany(order => order.Items)
                    .Sum(item => (decimal?)item.FinalPrice) ?? 0m,

                Orders = customer.Orders
                    .Select(order => new OrderDto
                    {
                        Id = order.Id,
                        Status = order.Status.ToString(),
                        OrderDate = order.OrderDate,
                        RequiredDate = order.RequiredDate,
                        ShippedDate = order.ShippedDate,

                        TotalOrder = order.Items
                            .Sum(item => (decimal?)item.FinalPrice) ?? 0m,

                        Items = order.Items
                            .Select(item => new OrderItemDto
                            {
                                Id = item.Id,
                                ItemId = item.ItemId,
                                ListPrice = item.ListPrice,
                                FinalPrice = item.FinalPrice,
                                Discount = item.Discount
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToListAsync(ct);
    }
}
