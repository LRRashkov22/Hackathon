using Passalong.Core.Enums;

namespace Passalong.Application.DTOs.Listings;

public sealed record BrowseListingsQuery(
    string? Search,
    Guid? CategoryId,
    ExchangeType? ExchangeType,
    ListingCondition? Condition,
    int Page,
    int PageSize);
