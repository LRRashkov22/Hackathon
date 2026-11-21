using Passalong.Core.Enums;

namespace Passalong.Application.DTOs.WantedItems;

public record CreateWantedItemRequest(
    Guid CategoryId,
    string Title,
    string Description,
    ListingCondition? PreferredCondition,
    string Location);
