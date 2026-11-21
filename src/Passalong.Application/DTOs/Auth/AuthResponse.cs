namespace Passalong.Application.DTOs.Auth;

public sealed record AuthResponse(string Token, AuthUserDto User);
