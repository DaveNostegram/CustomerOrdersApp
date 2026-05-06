using CustomerOrdersApp.Domain.Enum;
using CustomerOrdersApp.Domain.Orders;

namespace CustomerOrdersApp.Domain.Discounts;

public class Discount : BaseEntity
{
    public decimal DiscountAmount { get; set; }
    public DiscountTypeEnum Type { get; set; }
    public string Value { get; set; } = string.Empty;
}