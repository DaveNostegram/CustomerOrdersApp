using CustomerOrdersApp.Domain;
using CustomerOrdersApp.Domain.Customers;
using CustomerOrdersApp.Domain.Enum;

namespace CustomerOrdersApp.Domain.Orders;

public class Order : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public OrderStatusEnum Status { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
}