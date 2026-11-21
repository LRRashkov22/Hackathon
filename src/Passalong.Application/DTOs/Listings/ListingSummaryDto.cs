using Passalong.Core.Enums;

namespace Passalong.Application.DTOs.Listings;

public sealed record ListingSummaryDto(
    Guid Id,
    string Title,
    string Description,
    ListingCondition Condition,
    ExchangeType ExchangeType,
    string Location,
    DateTime CreatedAt,
    string Category,
    ListingOwnerDto Owner,
    IEnumerable<string> Images);
