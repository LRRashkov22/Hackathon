using Passalong.Core.Enums;

namespace Passalong.Core.Entities;

public class Listing
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OwnerId { get; set; }

    public Guid CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ListingCondition Condition { get; set; }

    public ExchangeType ExchangeType { get; set; }

    public string? DesiredSwap { get; set; }

    public ListingStatus Status { get; set; } = ListingStatus.Available;

    public string Location { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser Owner { get; set; } = null!;

    public Category Category { get; set; } = null!;

    public List<ListingImage> Images { get; set; } = [];
}
