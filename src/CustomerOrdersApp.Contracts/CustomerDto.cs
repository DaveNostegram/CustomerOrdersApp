namespace CustomerOrdersApp.Contracts;

public class CustomerDto
{
    public int Id { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }

    public required string Street { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string ZipCode { get; set; }

    public string? PhoneNumber { get; set; }

    public decimal TotalSpend { get; set; }

    public List<OrderDto> Orders { get; set; } = [];
}