using Microsoft.AspNetCore.Mvc;
using Passalong.Application.DTOs.Community;
using Passalong.Application.Services.Contracts;

namespace Passalong.Api.Controllers;

[ApiController]
[Route("api")]
public class CommunityController(ICommunityService community) : ApiControllerBase
{
    [HttpGet("impact/summary")]
    public Task<ImpactSummaryDto> Impact()
    {
        return community.GetImpactSummaryAsync();
    }

    [HttpGet("leaderboard")]
    public Task<List<LeaderboardEntryDto>> Leaderboard()
    {
        return community.GetLeaderboardAsync();
    }

    [HttpGet("items/{id:guid}/history")]
    public async Task<IActionResult> History(Guid id)
    {
        return Ok(await community.GetItemHistoryAsync(id));
    }
}
