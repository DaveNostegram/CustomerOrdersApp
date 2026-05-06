namespace CustomerOrdersApp.Application.Discounts;

public class DiscountResult
{
    public bool HasDiscount { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = "";
}