namespace CustomerOrdersApp.Contracts;

public class OrderDto
{
    public int Id { get; set; }

    public required string Status { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }

    public decimal TotalOrder { get; set; }

    public List<OrderItemDto> Items { get; set; } = [];
}