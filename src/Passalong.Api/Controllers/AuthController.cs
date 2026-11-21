using Microsoft.AspNetCore.Mvc;
using Passalong.Application.DTOs.Auth;
using Passalong.Application.Services.Contracts;

namespace Passalong.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest x)
    {
        var result = await auth.RegisterAsync(x);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest x)
    {
        var response = await auth.LoginAsync(x);

        if (response is null)
        {
            return Unauthorized();
        }

        return Ok(response);
    }
}
