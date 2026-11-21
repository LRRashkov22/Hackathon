using Passalong.Application.DTOs.Users;

namespace Passalong.Application.Services.Contracts;

public interface IUserService
{
    /// <summary>
    /// Returns null when the user does not exist.
    /// </summary>
    Task<UserProfileDto?> GetProfileAsync(Guid id);
}
