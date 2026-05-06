using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CustomerOrdersApp.Application.FileUploads.ImportCustomers.Validation;
using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Contracts;
using CustomerOrdersApp.Domain.Customers;
using CustomerOrdersApp.Domain.Enum;
using MediatR;

namespace CustomerOrdersApp.Application.FileUploads.ImportCustomers;

public sealed record ImportCustomersCommand(
    Stream FileStream,
    string FileName,
    string ContentType
) : IRequest<ImportCustomersCommandResult>;

public sealed class ImportCustomersCommandResult
{
    public bool Success => Errors.Count == 0;

    public int TotalRows { get; set; }
    public int FailedRows => Errors.Count;

    public List<string> Errors { get; set; } = [];
}

public sealed class ImportCustomersCommandHandler
    : IRequestHandler<ImportCustomersCommand, ImportCustomersCommandResult>
{
    private readonly ICustomerRepo _customerRepo;
    public ImportCustomersCommandHandler(
        ICustomerRepo customerRepo)
    {
        _customerRepo = customerRepo;
    }
    public async Task<ImportCustomersCommandResult> Handle(
        ImportCustomersCommand request,
        CancellationToken ct)
    {
        var result = new ImportCustomersCommandResult();

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
            CustomerUploadHeaders.Required);

        if (!headerValidation.IsValid)
        {
            foreach (var missingHeader in headerValidation.MissingHeaders)
            {
                result.Errors.Add($"Missing required header '{missingHeader}'.");
            }

            foreach (var dupHeader in headerValidation.DuplicateHeaders)
            {
                result.Errors.Add($"Duplicated header '{dupHeader}'.");
            }

            return result;
        }

        var customers = new List<Customer>();
        var validator = new CustomerImportRowValidator();
        var rowNumber = 1;

        while (await csv.ReadAsync())
        {
            rowNumber++;

            var row = FileUploadMapper.MapCustomerRow(csv, rowNumber);

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

            customers.Add(MapToCustomer(row));
        }

        result.TotalRows = rowNumber - 1;
        await _customerRepo.StageCustomers(customers, ct);
        await _customerRepo.SaveChangesAsync(ct);
        return result;
    }
    private static Customer MapToCustomer(CustomerImportRow row)
    {
        return new Customer
        {
            PublicId = int.Parse(row.CustomerId),

            FirstName = row.FirstName,
            LastName = row.LastName,
            Email = row.Email,

            Street = row.Street,
            City = row.City,
            State = Enum.Parse<StateEnum>(row.State, ignoreCase: true),
            ZipCode = row.ZipCode,

            PhoneNumber = !string.IsNullOrWhiteSpace(row.Phone) ? row.Phone : null
        };
    }
}
