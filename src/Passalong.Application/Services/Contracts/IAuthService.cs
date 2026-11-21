using Passalong.Application.DTOs.Auth;

namespace Passalong.Application.Services.Contracts;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Returns null when the credentials are invalid.
    /// </summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
