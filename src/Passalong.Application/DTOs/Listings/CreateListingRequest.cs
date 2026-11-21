using Passalong.Core.Enums;

namespace Passalong.Application.DTOs.Listings;

public record CreateListingRequest(
    Guid CategoryId,
    string Title,
    string Description,
    ListingCondition Condition,
    ExchangeType ExchangeType,
    string? DesiredSwap,
    string Location,
    List<string>? ImageUrls);
