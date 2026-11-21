using Microsoft.Extensions.Configuration;

namespace Passalong.Infrastructure.Authentication;

/// <summary>
/// JWT settings shared by token issuing (Infrastructure) and token validation (Api).
/// </summary>
public sealed record JwtSettings(string Key, string Issuer, string Audience)
{
    private const string DefaultKey = "development-only-change-this-key-to-a-secure-secret-32";

    private const string DefaultIssuer = "passalong";

    private const string DefaultAudience = "passalong";

    public static JwtSettings FromConfiguration(IConfiguration configuration)
    {
        return new JwtSettings(
            ValueOrDefault(configuration["Jwt:Key"], DefaultKey),
            ValueOrDefault(configuration["Jwt:Issuer"], DefaultIssuer),
            ValueOrDefault(configuration["Jwt:Audience"], DefaultAudience));
    }

    private static string ValueOrDefault(string? value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }
}
