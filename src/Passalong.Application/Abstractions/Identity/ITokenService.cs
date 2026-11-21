using Passalong.Core.Entities;

namespace Passalong.Application.Abstractions.Identity;

public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}
