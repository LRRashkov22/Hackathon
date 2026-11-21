namespace Passalong.Application.DTOs.Auth;

public sealed record AuthUserDto(Guid Id, string DisplayName, string? Email);
