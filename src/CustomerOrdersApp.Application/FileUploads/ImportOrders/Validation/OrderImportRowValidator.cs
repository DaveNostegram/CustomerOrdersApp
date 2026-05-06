using CustomerOrdersApp.Contracts;
using CustomerOrdersApp.Domain.Enum;
using FluentValidation;
using System.Globalization;

namespace CustomerOrdersApp.Application.FileUploads.ImportOrders.Validation;

public sealed class OrderImportRowValidator
{
    private readonly InlineValidator<OrderImportRow> _validator = new();
    private const string DateFormat = "dd/MM/yyyy";

    public OrderImportRowValidator(IReadOnlySet<int> validCustomerPublicIds)
    {
        _validator.RuleFor(x => x.CustomerId)
            .NotEmpty()
            .Must(x => int.TryParse(x, out _))
            .WithMessage("customer_id must be a valid integer.")
            .Must(x =>
                int.TryParse(x, out var id) &&
                validCustomerPublicIds.Contains(id))
            .WithMessage("customer_id does not exist.");

        _validator.RuleFor(x => x.OrderStatus)
        .NotEmpty()
        .Must(x => int.TryParse(x, out _))
        .WithMessage("order_status must be a valid integer.")
        .Must(x =>
            int.TryParse(x, out var status) &&
            Enum.IsDefined(typeof(OrderStatusEnum), status))
        .WithMessage("order_status is not valid.");

        _validator.RuleFor(x => x.OrderDate)
            .NotEmpty()
            .Must(BeValidDate)
            .WithMessage($"order_date must be in format {DateFormat}.");

        _validator.RuleFor(x => x.RequiredDate)
            .NotEmpty()
            .Must(BeValidDate)
            .WithMessage($"required_date must be in format {DateFormat}.");

        _validator.RuleFor(x => x.ShippedDate)
            .Must(BeValidOptionalDate)
            .WithMessage($"shipped_date must be in format {DateFormat}.")
            .When(x => !string.IsNullOrWhiteSpace(x.ShippedDate));
    }
    public Task<FluentValidation.Results.ValidationResult> ValidateAsync(
OrderImportRow row,
CancellationToken cancellationToken)
    {
        return _validator.ValidateAsync(row, cancellationToken);
    }
    private static bool BeValidInt(string value)
        => int.TryParse(value, out _);

    private static bool BeValidDate(string value)
        => DateTime.TryParseExact(
            value,
            DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);

    private static bool BeValidOptionalDate(string? value)
        => string.IsNullOrWhiteSpace(value) || BeValidDate(value);
}