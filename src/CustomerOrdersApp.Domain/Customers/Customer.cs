using CustomerOrdersApp.Domain;
using CustomerOrdersApp.Domain.Enum;
using CustomerOrdersApp.Domain.Orders;

namespace CustomerOrdersApp.Domain.Customers;

public class Customer : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }

    public required string Street { get; set; }
    public required string City { get; set; }
    public StateEnum State { get; set; }
    public required string ZipCode { get; set; }

    public string? PhoneNumber { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
}