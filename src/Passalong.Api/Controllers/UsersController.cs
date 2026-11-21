using Microsoft.AspNetCore.Mvc;
using Passalong.Application.Services.Contracts;

namespace Passalong.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService users) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var profile = await users.GetProfileAsync(id);

        return profile is null ? NotFound() : Ok(profile);
    }
}
