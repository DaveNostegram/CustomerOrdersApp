namespace CustomerOrdersApp.Contracts;

public sealed class CustomerImportRow
{
    public int RowNumber { get; init; }

    public required string CustomerId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Phone { get; init; }
    public required string Email { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string ZipCode { get; init; }
}
