using Microsoft.AspNetCore.Identity;

namespace Passalong.Core.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? AvatarUrl { get; set; }
    public int ReputationScore { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
