using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Passalong.Application.DTOs.Listings;
using Passalong.Application.Services.Contracts;
using Passalong.Core.Entities;
using Passalong.Core.Enums;

namespace Passalong.Api.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingsController(IListingService listings) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Browse(
        string? search,
        Guid? categoryId,
        ExchangeType? exchangeType,
        ListingCondition? condition,
        int page = 1,
        int pageSize = 12)
    {
        var query = new BrowseListingsQuery(search, categoryId, exchangeType, condition, page, pageSize);

        return Ok(await listings.BrowseAsync(query, OptionalCurrentUserId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var x = await listings.GetAsync(id);

        return x is null ? NotFound() : Ok(x);
    }

    [Authorize]
    [HttpGet("mine")]
    public Task<List<Listing>> Mine()
    {
        return listings.GetMineAsync(CurrentUserId);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateListingRequest x)
    {
        var result = await listings.CreateAsync(CurrentUserId, x);

        if (!result.IsSuccess)
        {
            return Failure(result.Error);
        }

        return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value);
    }
}
