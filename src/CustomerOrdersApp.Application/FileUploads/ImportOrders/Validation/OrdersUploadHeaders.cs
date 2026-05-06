namespace CustomerOrdersApp.Application.FileUploads.ImportOrders.Validation;

public static class OrdersUploadHeaders
{
    public const string OrderId = "order_id";
    public const string CustomerId = "customer_id";
    public const string OrderStatus = "order_status";
    public const string OrderDate = "order_date";
    public const string RequiredDate = "required_date";
    public const string ShippedDate = "shipped_date";
    public static readonly string[] Required =
    [
        OrderId,
        CustomerId,
    OrderStatus,
OrderDate,
RequiredDate,
    ];

    public static readonly string[] Optional =
    [
        ShippedDate
    ];
}