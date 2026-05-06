using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Domain.Customers;
using CustomerOrdersApp.Domain.Discounts;
using CustomerOrdersApp.Domain.Enum;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrdersApp.Infrastructure.Repositories;

public class DiscountRepo(AppDbContext context) : IDiscountRepo
{
    private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<Discount>> GetDiscounts(CancellationToken ct)
    {
        return new List<Discount>
    {
        new Discount
        {
            Type = DiscountTypeEnum.State,
            DiscountAmount = 0.05m,
            Value = "CA"
        }
    };

    }
    public async Task ApplyDiscountToUnshippedOrders(Customer customer, decimal discountRate, CancellationToken ct)
    {
        var customerOrderItems = await _context.Orders.Where(e => e.CustomerId == customer.Id && e.ShippedDate == null).SelectMany(e => e.Items).ToListAsync(ct);

        foreach (var customerOrderItem in customerOrderItems)
        {
            customerOrderItem.Discount = discountRate;
            customerOrderItem.FinalPrice = customerOrderItem.ListPrice * (1 - discountRate);
        }
    }
    public async Task ApplyDiscountToAllOrders(Customer customer, decimal discountRate, CancellationToken ct)
    {
        var customerOrderItems = await _context.Orders.Where(e => e.CustomerId == customer.Id).SelectMany(e => e.Items).ToListAsync(ct);

        foreach (var customerOrderItem in customerOrderItems)
        {
            customerOrderItem.Discount = discountRate;
            customerOrderItem.FinalPrice = customerOrderItem.ListPrice * (1 - discountRate);
        }
    }
}