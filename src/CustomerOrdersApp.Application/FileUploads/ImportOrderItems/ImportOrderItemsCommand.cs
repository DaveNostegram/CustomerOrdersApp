using CsvHelper;
using CsvHelper.Configuration;
using CustomerOrdersApp.Application.FileUploads.ImportOrders.Validation;
using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Contracts;
using CustomerOrdersApp.Domain.Orders;
using MediatR;
using System.Globalization;

namespace CustomerOrdersApp.Application.FileUploads.Commands;

public sealed record ImportOrderItemsCommand(
    Stream FileStream,
    string FileName,
    string ContentType
) : IRequest<ImportOrderItemsCommandResult>;

public sealed class ImportOrderItemsCommandResult
{
    public bool Success => Errors.Count == 0;

    public int TotalRows { get; set; }
    public int FailedRows => Errors.Count;

    public List<string> Errors { get; set; } = [];
}

public sealed class ImportOrderItemsCommandHandler
    : IRequestHandler<ImportOrderItemsCommand, ImportOrderItemsCommandResult>
{
    private readonly ICustomerRepo _customerRepo;

    public ImportOrderItemsCommandHandler(ICustomerRepo customerRepo)
    {
        _customerRepo = customerRepo;
    }

    public async Task<ImportOrderItemsCommandResult> Handle(
        ImportOrderItemsCommand request,
        CancellationToken ct)
    {
        var result = new ImportOrderItemsCommandResult();

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
            OrderItemsUploadHeaders.Required);

        if (!headerValidation.IsValid)
        {
            foreach (var missingHeader in headerValidation.MissingHeaders)
                result.Errors.Add($"Missing required header '{missingHeader}'.");

            foreach (var dupHeader in headerValidation.DuplicateHeaders)
                result.Errors.Add($"Duplicated header '{dupHeader}'.");

            return result;
        }

        var importRows = new List<OrderItemImportRow>();
        var rowNumber = 1;

        while (await csv.ReadAsync())
        {
            rowNumber++;
            importRows.Add(FileUploadMapper.MapOrderItemRow(csv, rowNumber));
        }

        result.TotalRows = importRows.Count;

        var requestedOrderPublicIds = importRows
            .Select(x => int.TryParse(x.OrderId, out var id) ? id : (int?)null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        var ordersByPublicId = await _customerRepo.GetOrderIdsByPublicIdsAsync(
            requestedOrderPublicIds,
            ct);

        var validOrderPublicIds = ordersByPublicId.Keys.ToHashSet();

        var orderItems = new List<OrderItem>();
        var validator = new OrderItemImportRowValidator(validOrderPublicIds);

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

            orderItems.Add(MapToOrderItem(row, ordersByPublicId));
        }

        await _customerRepo.StageOrderItems(orderItems, ct);
        await _customerRepo.SaveChangesAsync(ct);

        return result;
    }

    private static OrderItem MapToOrderItem(
        OrderItemImportRow row,
        IReadOnlyDictionary<int, int> orderDbIdsByPublicId)
    {
        var orderPublicId = int.Parse(row.OrderId);
        var discount = !string.IsNullOrWhiteSpace(row.Discount)
            ? decimal.Parse(row.Discount, CultureInfo.InvariantCulture)
            : 0.00m;

        var listPrice = decimal.Parse(row.ListPrice, CultureInfo.InvariantCulture);

        return new OrderItem
        {
            ItemId = int.Parse(row.ItemId),
            OrderId = orderDbIdsByPublicId[orderPublicId],
            ListPrice = listPrice,
            Discount = discount,
            FinalPrice = listPrice * (1 - discount)
        };
    }
}