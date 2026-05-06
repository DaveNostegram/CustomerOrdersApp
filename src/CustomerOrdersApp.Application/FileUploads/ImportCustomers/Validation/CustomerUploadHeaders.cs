namespace CustomerOrdersApp.Application.FileUploads.ImportCustomers.Validation;

public static class CustomerUploadHeaders
{
    public const string CustomerId = "customer_id";
    public const string FirstName = "first_name";
    public const string LastName = "last_name";
    public const string Phone = "phone";
    public const string Email = "email";
    public const string Street = "street";
    public const string City = "city";
    public const string State = "state";
    public const string ZipCode = "zip_code";

    public static readonly string[] Required =
    [
        CustomerId,
    FirstName,
    LastName,
    Email,
    Street,
    City,
    State,
    ZipCode
    ];

    public static readonly string[] Optional =
    [
        Phone
    ];
}