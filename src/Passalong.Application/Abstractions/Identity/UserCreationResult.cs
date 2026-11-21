using Passalong.Application.DTOs.Auth;

namespace Passalong.Application.Abstractions.Identity;

public sealed record UserCreationResult(bool Succeeded, IReadOnlyList<IdentityErrorDto> Errors);
