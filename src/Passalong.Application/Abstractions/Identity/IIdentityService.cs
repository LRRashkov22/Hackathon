using Passalong.Core.Entities;

namespace Passalong.Application.Abstractions.Identity;

public interface IIdentityService
{
    Task<UserCreationResult> CreateUserAsync(ApplicationUser user, string password);

    /// <summary>
    /// Returns the user when the email exists and the password matches; otherwise null.
    /// </summary>
    Task<ApplicationUser?> ValidateCredentialsAsync(string email, string password);
}
