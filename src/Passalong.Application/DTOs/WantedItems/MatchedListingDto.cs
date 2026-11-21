using Passalong.Core.Enums;

namespace Passalong.Application.DTOs.WantedItems;

public sealed record MatchedListingDto(
    Guid Id,
    string Title,
    string Description,
    string Location,
    ListingCondition Condition,
    ExchangeType ExchangeType,
    string Category,
    string Owner);
