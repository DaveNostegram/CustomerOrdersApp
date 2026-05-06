using CustomerOrdersApp.Domain;

namespace CustomerOrdersApp.Domain.Orders;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ItemId { get; set; }
    public decimal ListPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal Discount { get; set; }
}