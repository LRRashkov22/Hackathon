using Passalong.Application.Abstractions.Identity;
using Passalong.Application.DTOs.Auth;
using Passalong.Application.Services.Contracts;
using Passalong.Core.Entities;

namespace Passalong.Application.Services.Implementation;

public class AuthService(IIdentityService identity, ITokenService tokens) : IAuthService
{
    public async Task<RegisterResult> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            Location = request.Location
        };
        var result = await identity.CreateUserAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new RegisterResult(null, result.Errors);
        }

        return new RegisterResult(CreateResponse(user), []);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await identity.ValidateCredentialsAsync(request.Email, request.Password);

        if (user is null)
        {
            return null;
        }

        return CreateResponse(user);
    }

    private AuthResponse CreateResponse(ApplicationUser user)
    {
        return new AuthResponse(
            tokens.CreateToken(user),
            new AuthUserDto(user.Id, user.DisplayName, user.Email));
    }
}
