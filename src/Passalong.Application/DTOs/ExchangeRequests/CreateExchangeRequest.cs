namespace Passalong.Application.DTOs.ExchangeRequests;

public record CreateExchangeRequest(
    Guid ListingId,
    Guid? OfferedListingId,
    string? Message);
