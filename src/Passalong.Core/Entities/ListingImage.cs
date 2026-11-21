namespace Passalong.Core.Entities;

public class ListingImage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ListingId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public Listing Listing { get; set; } = null!;
}
