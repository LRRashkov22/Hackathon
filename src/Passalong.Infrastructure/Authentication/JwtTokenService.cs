using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Passalong.Application.Abstractions.Identity;
using Passalong.Core.Entities;

namespace Passalong.Infrastructure.Authentication;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromHours(8);

    public string CreateToken(ApplicationUser user)
    {
        var settings = JwtSettings.FromConfiguration(configuration);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.DisplayName)
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            settings.Issuer,
            settings.Audience,
            claims,
            expires: DateTime.UtcNow.Add(Lifetime),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
