namespace CustomerOrdersApp.Contracts;

public class OrderItemDto
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public decimal ListPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal Discount { get; set; }
}