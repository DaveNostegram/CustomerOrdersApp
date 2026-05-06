using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Contracts;
using MediatR;

namespace CustomerOrdersApp.Application.Discounts.ApplyDiscount;

public sealed record GetCustomerQuery() : IRequest<GetCustomerQueryResult>;

public sealed class GetCustomerQueryResult
{
    public bool Success => Errors.Count == 0;
    public List<CustomerDto>? CustomerDtos { get; set; }
    public List<string> Errors { get; init; } = [];
}

public sealed class GetCustomerQueryHandler
    : IRequestHandler<GetCustomerQuery, GetCustomerQueryResult>
{
    private readonly ICustomerRepo _customerRepo;
    public GetCustomerQueryHandler(ICustomerRepo customerRepo)
    {
        _customerRepo = customerRepo;
    }
    public async Task<GetCustomerQueryResult> Handle(
        GetCustomerQuery request,
        CancellationToken ct)
    {
        var customers = await _customerRepo.GetAllCustomerDtos(ct);

        return new GetCustomerQueryResult { CustomerDtos = customers };
    }
}
