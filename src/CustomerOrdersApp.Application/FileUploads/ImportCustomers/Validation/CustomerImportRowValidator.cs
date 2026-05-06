using CustomerOrdersApp.Contracts;
using CustomerOrdersApp.Domain.Enum;
using FluentValidation;

namespace CustomerOrdersApp.Application.FileUploads.ImportCustomers.Validation;

public sealed class CustomerImportRowValidator
: AbstractValidator<CustomerImportRow>
{
    public CustomerImportRowValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .Must(x => int.TryParse(x, out _))
            .WithMessage("customer_id must be a valid integer.");

        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Street).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.State)
            .NotEmpty()
            .Must(x =>
                Enum.IsDefined(typeof(StateEnum), x))
            .WithMessage("state is not valid.");
        RuleFor(x => x.ZipCode).NotEmpty();

        RuleFor(x => x.Phone)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
