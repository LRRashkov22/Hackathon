using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Passalong.Application.Common;

namespace Passalong.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// The signed-in user's id. Only valid on actions that require authorization.
    /// </summary>
    protected Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// The signed-in user's id, or null for anonymous requests.
    /// </summary>
    protected Guid? OptionalCurrentUserId
    {
        get
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    protected IActionResult Failure(Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => BadRequest(error.Message),
            ErrorType.Conflict => Conflict(error.Message),
            ErrorType.NotFound => NotFound(),
            ErrorType.Forbidden => Forbid(),
            _ => throw new ArgumentOutOfRangeException(nameof(error), error.Type, "Unknown error type.")
        };
    }
}
