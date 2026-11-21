namespace Passalong.Application.DTOs.Listings;

public sealed record ListingPageDto(
    int Total,
    int Page,
    int PageSize,
    List<ListingSummaryDto> Items);
