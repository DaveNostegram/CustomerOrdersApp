using CsvHelper;
using CustomerOrdersApp.Application.FileUploads.ImportCustomers.Validation;
using CustomerOrdersApp.Application.FileUploads.ImportOrders.Validation;
using CustomerOrdersApp.Contracts;

namespace CustomerOrdersApp.Application.FileUploads;

public static class FileUploadMapper
{
    public static CustomerImportRow MapCustomerRow(
    CsvReader csv,
    int rowNumber)
    {
        return new CustomerImportRow
        {
            RowNumber = rowNumber,

            CustomerId = GetField(csv, CustomerUploadHeaders.CustomerId),
            FirstName = GetField(csv, CustomerUploadHeaders.FirstName),
            LastName = GetField(csv, CustomerUploadHeaders.LastName),
            Phone = GetOptionalField(csv, CustomerUploadHeaders.Phone),
            Email = GetField(csv, CustomerUploadHeaders.Email),
            Street = GetField(csv, CustomerUploadHeaders.Street),
            City = GetField(csv, CustomerUploadHeaders.City),
            State = GetField(csv, CustomerUploadHeaders.State),
            ZipCode = GetField(csv, CustomerUploadHeaders.ZipCode)
        };
    }

    public static OrderImportRow MapOrderRow(
        CsvReader csv,
        int rowNumber)
    {
        return new OrderImportRow
        {
            RowNumber = rowNumber,

            OrderId = GetField(csv, OrdersUploadHeaders.OrderId),
            CustomerId = GetField(csv, OrdersUploadHeaders.CustomerId),
            OrderStatus = GetField(csv, OrdersUploadHeaders.OrderStatus),
            OrderDate = GetField(csv, OrdersUploadHeaders.OrderDate),
            RequiredDate = GetField(csv, OrdersUploadHeaders.RequiredDate),
            ShippedDate = GetOptionalField(csv, OrdersUploadHeaders.ShippedDate),
        };
    }

    public static OrderItemImportRow MapOrderItemRow(
        CsvReader csv,
        int rowNumber)
    {
        return new OrderItemImportRow
        {
            RowNumber = rowNumber,

            OrderId = GetField(csv, OrderItemsUploadHeaders.OrderId),
            ItemId = GetField(csv, OrderItemsUploadHeaders.ItemId),
            ListPrice = GetField(csv, OrderItemsUploadHeaders.ListPrice),
            Discount = GetOptionalField(csv, OrderItemsUploadHeaders.Discount),
        };
    }

    private static string GetField(
        CsvReader csv,
        string headerName)
    {
        return csv.GetField(headerName)?.Trim() ?? string.Empty;
    }

    private static string? GetOptionalField(
        CsvReader csv,
        string headerName)
    {
        var value = csv.GetField(headerName)?.Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }
}