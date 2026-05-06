namespace CustomerOrdersApp.Contracts;

public sealed class OrderItemImportRow
{
    public int RowNumber { get; init; }
    public required string OrderId { get; init; }
    public required string ItemId { get; init; }
    public required string ListPrice { get; init; }
    public string? Discount { get; init; }


}
