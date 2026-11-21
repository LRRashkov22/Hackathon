using Passalong.Core.Enums;

namespace Passalong.Core.Entities;

public class ExchangeRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ListingId { get; set; }

    public Guid RequesterId { get; set; }

    public Guid OwnerId { get; set; }

    public Guid? OfferedListingId { get; set; }

    public string? Message { get; set; }

    public ExchangeRequestStatus Status { get; set; } = ExchangeRequestStatus.Pending;

    public bool RequesterConfirmed { get; set; }

    public bool OwnerConfirmed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Listing Listing { get; set; } = null!;

    public ApplicationUser Requester { get; set; } = null!;

    public ApplicationUser Owner { get; set; } = null!;
}
