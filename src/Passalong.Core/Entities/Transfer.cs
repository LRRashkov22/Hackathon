namespace Passalong.Core.Entities;

public class Transfer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ListingId { get; set; }

    public Guid PreviousOwnerId { get; set; }

    public Guid NewOwnerId { get; set; }

    public Guid ExchangeRequestId { get; set; }

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}
