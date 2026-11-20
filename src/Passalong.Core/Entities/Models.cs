using Passalong.Core.Enums;

namespace Passalong.Core.Entities;

public class Category { public Guid Id { get; set; } = Guid.NewGuid(); public string Name { get; set; } = string.Empty; public string? Description { get; set; } public string? Icon { get; set; } }
public class Listing
{
    public Guid Id { get; set; } = Guid.NewGuid(); public Guid OwnerId { get; set; } public Guid CategoryId { get; set; }
    public string Title { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; public ListingCondition Condition { get; set; }
    public ExchangeType ExchangeType { get; set; } public string? DesiredSwap { get; set; } public ListingStatus Status { get; set; } = ListingStatus.Available; public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ApplicationUser Owner { get; set; } = null!; public Category Category { get; set; } = null!; public List<ListingImage> Images { get; set; } = [];
}
public class ListingImage { public Guid Id { get; set; } = Guid.NewGuid(); public Guid ListingId { get; set; } public string ImageUrl { get; set; } = string.Empty; public int DisplayOrder { get; set; } public Listing Listing { get; set; } = null!; }
public class WantedItem { public Guid Id { get; set; } = Guid.NewGuid(); public Guid UserId { get; set; } public Guid CategoryId { get; set; } public string Title { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; public ListingCondition? PreferredCondition { get; set; } public string Location { get; set; } = string.Empty; public WantedItemStatus Status { get; set; } = WantedItemStatus.Active; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public ApplicationUser User { get; set; } = null!; public Category Category { get; set; } = null!; }
public class ExchangeRequest { public Guid Id { get; set; } = Guid.NewGuid(); public Guid ListingId { get; set; } public Guid RequesterId { get; set; } public Guid OwnerId { get; set; } public Guid? OfferedListingId { get; set; } public string? Message { get; set; } public ExchangeRequestStatus Status { get; set; } = ExchangeRequestStatus.Pending; public bool RequesterConfirmed { get; set; } public bool OwnerConfirmed { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; public Listing Listing { get; set; } = null!; public ApplicationUser Requester { get; set; } = null!; public ApplicationUser Owner { get; set; } = null!; }
public class Transfer { public Guid Id { get; set; } = Guid.NewGuid(); public Guid ListingId { get; set; } public Guid PreviousOwnerId { get; set; } public Guid NewOwnerId { get; set; } public Guid ExchangeRequestId { get; set; } public DateTime CompletedAt { get; set; } = DateTime.UtcNow; }
public class Review { public Guid Id { get; set; } = Guid.NewGuid(); public Guid TransferId { get; set; } public Guid ReviewerId { get; set; } public Guid ReviewedUserId { get; set; } public int Rating { get; set; } public string? Comment { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
