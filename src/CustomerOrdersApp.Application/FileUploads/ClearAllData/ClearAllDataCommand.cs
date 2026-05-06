using CustomerOrdersApp.Application.Interfaces.Repositories;
using MediatR;

namespace CustomerOrdersApp.Application.FileUploads.ImportCustomers;

public sealed record ClearAllDataCommand(
) : IRequest<ClearAllDataCommandResult>;

public sealed class ClearAllDataCommandResult
{
    public bool Success => Errors.Count == 0;
    public List<string> Errors { get; set; } = [];
}

public sealed class ClearAllDataCommandHandler
    : IRequestHandler<ClearAllDataCommand, ClearAllDataCommandResult>
{
    private readonly ICustomerRepo _customerRepo;
    public ClearAllDataCommandHandler(
        ICustomerRepo customerRepo)
    {
        _customerRepo = customerRepo;
    }
    public async Task<ClearAllDataCommandResult> Handle(
        ClearAllDataCommand request,
        CancellationToken ct)
    {
        await _customerRepo.ClearAllData(ct);
        await _customerRepo.SaveChangesAsync(ct);
        return new ClearAllDataCommandResult();
    }
}
