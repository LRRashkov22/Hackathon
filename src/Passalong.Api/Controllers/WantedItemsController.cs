using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Passalong.Application.DTOs.WantedItems;
using Passalong.Application.Services.Contracts;
using Passalong.Core.Entities;

namespace Passalong.Api.Controllers;

[ApiController]
[Route("api/wanted-items")]
public class WantedItemsController(IWantedItemService wantedItems) : ApiControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateWantedItemRequest x)
    {
        return Ok(await wantedItems.CreateAsync(CurrentUserId, x));
    }

    [Authorize]
    [HttpGet("mine")]
    public Task<List<WantedItem>> Mine()
    {
        return wantedItems.GetMineAsync(CurrentUserId);
    }

    [HttpGet("{id:guid}/matches")]
    public async Task<IActionResult> Matches(Guid id)
    {
        var matches = await wantedItems.GetMatchesAsync(id);

        return matches is null ? NotFound() : Ok(matches);
    }
}
