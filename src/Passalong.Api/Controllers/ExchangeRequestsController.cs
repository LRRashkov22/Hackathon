using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Passalong.Application.DTOs.ExchangeRequests;
using Passalong.Application.Services.Contracts;
using Passalong.Core.Entities;

namespace Passalong.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/exchange-requests")]
public class ExchangeRequestsController(IExchangeRequestService exchanges) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateExchangeRequest request)
    {
        var result = await exchanges.CreateAsync(CurrentUserId, request);

        if (!result.IsSuccess)
        {
            return Failure(result.Error);
        }

        return Created($"api/exchange-requests/{result.Value.Id}", result.Value);
    }

    [HttpGet("incoming")]
    public Task<List<ExchangeRequest>> Incoming()
    {
        return exchanges.GetIncomingAsync(CurrentUserId);
    }

    [HttpGet("outgoing")]
    public Task<List<ExchangeRequest>> Outgoing()
    {
        return exchanges.GetOutgoingAsync(CurrentUserId);
    }

    [HttpPost("{id:guid}/accept")]
    public async Task<IActionResult> Accept(Guid id)
    {
        var result = await exchanges.AcceptAsync(CurrentUserId, id);

        return result.IsSuccess ? Ok(result.Value) : Failure(result.Error);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var result = await exchanges.RejectAsync(CurrentUserId, id);

        return result.IsSuccess ? Ok(result.Value) : Failure(result.Error);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await exchanges.CancelAsync(CurrentUserId, id);

        return result.IsSuccess ? Ok(result.Value) : Failure(result.Error);
    }

    [HttpPost("{id:guid}/confirm-handover")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var result = await exchanges.ConfirmHandoverAsync(CurrentUserId, id);

        return result.IsSuccess ? Ok(result.Value) : Failure(result.Error);
    }
}
