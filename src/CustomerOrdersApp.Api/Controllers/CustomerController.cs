using CustomerOrdersApp.Application.Discounts.ApplyDiscount;
using CustomerOrdersApp.Application.FileUploads.Commands;
using CustomerOrdersApp.Application.FileUploads.ImportCustomers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerOrdersApp.Api.Controllers;

[Route("api/[controller]")]
public class CustomerController(ISender _sender) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _sender.Send(new GetCustomerQuery());

        return Ok(result);
    }
    [HttpPost("ApplyDiscounts")]
    public async Task<IActionResult> ApplyDiscounts(CancellationToken ct)
    {
        var result = await _sender.Send(new ApplyDiscountCommand());

        return Ok(result);
    }
}