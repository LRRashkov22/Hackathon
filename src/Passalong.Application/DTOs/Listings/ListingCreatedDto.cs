using Passalong.Core.Enums;

namespace Passalong.Application.DTOs.Listings;

public sealed record ListingCreatedDto(
    Guid Id,
    string Title,
    string Description,
    ListingCondition Condition,
    ExchangeType ExchangeType,
    string? DesiredSwap,
    string Location,
    ListingStatus Status,
    DateTime CreatedAt,
    IEnumerable<string> ImageUrls);
