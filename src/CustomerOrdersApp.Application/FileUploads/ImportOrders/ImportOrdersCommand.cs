using CsvHelper;
using CsvHelper.Configuration;
using CustomerOrdersApp.Application.FileUploads.ImportOrders.Validation;
using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Contracts;
using CustomerOrdersApp.Domain.Enum;
using CustomerOrdersApp.Domain.Orders;
using MediatR;
using System.Globalization;

namespace CustomerOrdersApp.Application.FileUploads.Commands;

public sealed record ImportOrdersCommand(
    Stream FileStream,
    string FileName,
    string ContentType
) : IRequest<ImportOrdersCommandResult>;

public sealed class ImportOrdersCommandResult
{
    public bool Success => Errors.Count == 0;

    public int TotalRows { get; set; }
    public int FailedRows => Errors.Count;

    public List<string> Errors { get; set; } = [];
}

public sealed class ImportOrdersCommandHandler
    : IRequestHandler<ImportOrdersCommand, ImportOrdersCommandResult>
{
    private readonly ICustomerRepo _customerRepo;

    public ImportOrdersCommandHandler(ICustomerRepo customerRepo)
    {
        _customerRepo = customerRepo;
    }

    public async Task<ImportOrdersCommandResult> Handle(
        ImportOrdersCommand request,
        CancellationToken ct)
    {
        var result = new ImportOrdersCommandResult();

        await using var memoryStream = new MemoryStream();
        await request.FileStream.CopyToAsync(memoryStream, ct);
        memoryStream.Position = 0;

        using var reader = new StreamReader(memoryStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            HeaderValidated = null
        });

        await csv.ReadAsync();
        csv.ReadHeader();

        var headers = csv.HeaderRecord?
            .Select(x => x.Trim())
            .ToList() ?? [];

        var headerValidation = FileHeaderValidator.ValidateHeaders(
            headers,
            OrdersUploadHeaders.Required);

        if (!headerValidation.IsValid)
        {
            foreach (var missingHeader in headerValidation.MissingHeaders)
                result.Errors.Add($"Missing required header '{missingHeader}'.");

            foreach (var dupHeader in headerValidation.DuplicateHeaders)
                result.Errors.Add($"Duplicated header '{dupHeader}'.");

            return result;
        }

        var importRows = new List<OrderImportRow>();
        var rowNumber = 1;

        while (await csv.ReadAsync())
        {
            rowNumber++;
            importRows.Add(FileUploadMapper.MapOrderRow(csv, rowNumber));
        }

        result.TotalRows = importRows.Count;

        var requestedCustomerPublicIds = importRows
            .Select(x => int.TryParse(x.CustomerId, out var id) ? id : (int?)null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        var customersByPublicId = await _customerRepo.GetCustomerIdsByPublicIdsAsync(
            requestedCustomerPublicIds,
            ct);

        var validCustomerPublicIds = customersByPublicId.Keys.ToHashSet();

        var orders = new List<Order>();
        var validator = new OrderImportRowValidator(validCustomerPublicIds);

        foreach (var row in importRows)
        {
            var validationResult = await validator.ValidateAsync(row, ct);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    result.Errors.Add(
                        $"Row '{row.RowNumber}' column '{error.PropertyName}': {error.ErrorMessage}");
                }

                continue;
            }

            orders.Add(MapToOrder(row, customersByPublicId));
        }

        await _customerRepo.StageOrders(orders, ct);
        await _customerRepo.SaveChangesAsync(ct);

        return result;
    }

    private static Order MapToOrder(
        OrderImportRow row,
        IReadOnlyDictionary<int, int> customerDbIdsByPublicId)
    {
        var customerPublicId = int.Parse(row.CustomerId);

        return new Order
        {
            PublicId = int.Parse(row.OrderId),
            CustomerId = customerDbIdsByPublicId[customerPublicId],

            Status = (OrderStatusEnum)int.Parse(row.OrderStatus),
            OrderDate = ParseUploadDate(row.OrderDate, nameof(row.OrderDate), row.RowNumber),

            RequiredDate = ParseUploadDate(row.RequiredDate, nameof(row.RequiredDate), row.RowNumber),
            ShippedDate = !string.IsNullOrWhiteSpace(row.ShippedDate)
                ? ParseUploadDate(row.ShippedDate, nameof(row.ShippedDate), row.RowNumber)
                : null,
        };
    }
    private static readonly string[] DateFormats =
        [
        "dd/MM/yyyy",
        ];

    private static DateTime ParseUploadDate(string value, string fieldName, int rowNumber)
    {
        if (DateTime.TryParseExact(
                value,
                DateFormats,
                CultureInfo.GetCultureInfo("en-GB"),
                DateTimeStyles.None,
                out var date))
        {
            return date;
        }

        throw new FormatException(
            $"Row {rowNumber}: '{fieldName}' has invalid date '{value}'. Expected dd/MM/yyyy.");
    }
}