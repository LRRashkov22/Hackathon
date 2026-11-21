using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Passalong.Application.DTOs.Reviews;
using Passalong.Application.Services.Contracts;

namespace Passalong.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/reviews")]
public class ReviewsController(IReviewService reviews) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewRequest x)
    {
        var result = await reviews.CreateAsync(CurrentUserId, x);

        return result.IsSuccess ? Ok() : Failure(result.Error);
    }
}
