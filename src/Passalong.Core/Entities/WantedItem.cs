using Passalong.Core.Enums;

namespace Passalong.Core.Entities;

public class WantedItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ListingCondition? PreferredCondition { get; set; }

    public string Location { get; set; } = string.Empty;

    public WantedItemStatus Status { get; set; } = WantedItemStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;

    public Category Category { get; set; } = null!;
}
