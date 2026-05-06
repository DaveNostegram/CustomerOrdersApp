using CustomerOrdersApp.Application.FileUploads.Commands;
using CustomerOrdersApp.Application.FileUploads.ImportCustomers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerOrdersApp.Api.Controllers;

[Route("api/[controller]")]
public class UploadFileController(ISender _sender) : Controller
{
    [HttpPost("customers")]
    public async Task<IActionResult> UploadCustomers([FromForm] IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();

        var result = await _sender.Send(new ImportCustomersCommand(
            stream,
            file.FileName,
            file.ContentType
        ));

        return Ok(result);
    }
    [HttpPost("orders")]
    public async Task<IActionResult> UploadOrders([FromForm] IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();

        var result = await _sender.Send(new ImportOrdersCommand(
            stream,
            file.FileName,
            file.ContentType
        ), ct);

        return Ok(result);
    }
    [HttpPost("order-items")]
    public async Task<IActionResult> UploadOrderItems([FromForm] IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();

        var result = await _sender.Send(new ImportOrderItemsCommand(
            stream,
            file.FileName,
            file.ContentType
        ), ct);

        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(CancellationToken ct)
    {
        var result = await _sender.Send(new ClearAllDataCommand(), ct);
        return Ok(result);
    }
}