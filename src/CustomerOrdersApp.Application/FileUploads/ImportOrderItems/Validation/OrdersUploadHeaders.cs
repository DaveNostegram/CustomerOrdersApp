namespace CustomerOrdersApp.Application.FileUploads.ImportOrders.Validation;

public static class OrderItemsUploadHeaders
{
    public const string OrderId = "order_id";
    public const string ItemId = "item_id";
    public const string ListPrice = "list_price";
    public const string Discount = "discount";
    public static readonly string[] Required =
    [
        OrderId,
        ItemId,
        ListPrice
    ];

    public static readonly string[] Optional =
    [
        Discount
    ];
}