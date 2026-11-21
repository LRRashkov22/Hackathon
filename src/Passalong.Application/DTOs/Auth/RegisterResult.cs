using System.Diagnostics.CodeAnalysis;

namespace Passalong.Application.DTOs.Auth;

public sealed record RegisterResult(AuthResponse? Response, IReadOnlyList<IdentityErrorDto> Errors)
{
    [MemberNotNullWhen(true, nameof(Response))]
    public bool Succeeded => Response is not null;
}
