using Passalong.Core.Enums;
namespace Passalong.Api.Contracts;
public record RegisterRequest(string Email, string Password, string DisplayName, string? Location);
public record LoginRequest(string Email, string Password);
public record CreateListingRequest(Guid CategoryId, string Title, string Description, ListingCondition Condition, ExchangeType ExchangeType, string? DesiredSwap, string Location, List<string>? ImageUrls);
public record CreateWantedItemRequest(Guid CategoryId, string Title, string Description, ListingCondition? PreferredCondition, string Location);
public record CreateExchangeRequest(Guid ListingId, Guid? OfferedListingId, string? Message);
