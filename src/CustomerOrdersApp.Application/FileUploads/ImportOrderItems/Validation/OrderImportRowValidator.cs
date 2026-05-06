using CustomerOrdersApp.Contracts;
using CustomerOrdersApp.Domain.Enum;
using FluentValidation;
using System.Globalization;

namespace CustomerOrdersApp.Application.FileUploads.ImportOrders.Validation;

public sealed class OrderItemImportRowValidator
{
    private readonly InlineValidator<OrderItemImportRow> _validator = new();
    public OrderItemImportRowValidator(IReadOnlySet<int> validOrderPublicIds)
    {
        _validator.RuleFor(x => x.OrderId)
            .NotEmpty()
            .Must(x => int.TryParse(x, out _))
            .WithMessage("order_Id must be a valid integer.")
            .Must(x =>
                int.TryParse(x, out var id) &&
                validOrderPublicIds.Contains(id))
            .WithMessage("order_Id does not exist.");

        _validator.RuleFor(x => x.ItemId)
            .NotEmpty()
            .Must(x => int.TryParse(x, out _))
            .WithMessage("item_id must be a valid integer.");

        _validator.RuleFor(x => x.ListPrice)
            .NotEmpty()
            .Must(x => decimal.TryParse(x, out _))
            .WithMessage("list_price must be a valid decimal.");
    }
    public Task<FluentValidation.Results.ValidationResult> ValidateAsync(
    OrderItemImportRow row,
    CancellationToken cancellationToken)
    {
        return _validator.ValidateAsync(row, cancellationToken);
    }
}