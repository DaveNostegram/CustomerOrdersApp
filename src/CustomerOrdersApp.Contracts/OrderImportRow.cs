namespace CustomerOrdersApp.Contracts;

public sealed class OrderImportRow
{
    public int RowNumber { get; init; }
    public required string OrderId { get; init; }
    public required string CustomerId { get; init; }
    public required string OrderStatus { get; init; }
    public required string OrderDate { get; init; }
    public required string RequiredDate { get; init; }
    public required string? ShippedDate { get; init; }
}
