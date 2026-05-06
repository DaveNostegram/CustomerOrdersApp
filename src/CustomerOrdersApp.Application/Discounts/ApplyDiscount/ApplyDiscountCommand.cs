using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Application.Interfaces.Services;
using MediatR;

namespace CustomerOrdersApp.Application.Discounts.ApplyDiscount;

public sealed record ApplyDiscountCommand(
) : IRequest<ApplyDiscountCommandResult>;

public sealed class ApplyDiscountCommandResult
{
    public bool Success => Errors.Count == 0;
    public List<string> Errors { get; init; } = [];
    public int DiscountsApplied { get; init; }
}

public sealed class ApplyDiscountCommandHandler
    : IRequestHandler<ApplyDiscountCommand, ApplyDiscountCommandResult>
{
    private readonly IDiscountService _discountService;
    private readonly ICustomerRepo _customerRepo;
    public ApplyDiscountCommandHandler(IDiscountService discountService, ICustomerRepo customerRepo)
    {
        _discountService = discountService;
        _customerRepo = customerRepo;
    }
    public async Task<ApplyDiscountCommandResult> Handle(
        ApplyDiscountCommand request,
        CancellationToken ct)
    {
        var customers = await _customerRepo.GetAllCustomers(ct);
        var discountsApplied = 0;
        foreach (var customer in customers)
        {
            var discount = await _discountService.ApplyDiscountAsync(customer, ct);
            if (discount.HasDiscount)
            {
                discountsApplied++;
            }
        }

        if (discountsApplied > 0)
        {
            await _customerRepo.SaveChangesAsync(ct);
        }

        return new ApplyDiscountCommandResult { DiscountsApplied = discountsApplied };
    }
}
