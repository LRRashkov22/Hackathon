using Microsoft.AspNetCore.Identity;
using Passalong.Application.Abstractions.Identity;
using Passalong.Application.DTOs.Auth;
using Passalong.Core.Entities;

namespace Passalong.Infrastructure.Identity;

public class IdentityService(UserManager<ApplicationUser> users) : IIdentityService
{
    public async Task<UserCreationResult> CreateUserAsync(ApplicationUser user, string password)
    {
        var result = await users.CreateAsync(user, password);
        var errors = result.Errors
            .Select(e => new IdentityErrorDto(e.Code, e.Description))
            .ToList();

        return new UserCreationResult(result.Succeeded, errors);
    }

    public async Task<ApplicationUser?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await users.FindByEmailAsync(email);

        if (user is null || !await users.CheckPasswordAsync(user, password))
        {
            return null;
        }

        return user;
    }
}
