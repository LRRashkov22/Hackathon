using Passalong.Application.DTOs.WantedItems;
using Passalong.Core.Entities;

namespace Passalong.Application.Services.Contracts;

public interface IWantedItemService
{
    Task<WantedItem> CreateAsync(Guid userId, CreateWantedItemRequest request);

    Task<List<WantedItem>> GetMineAsync(Guid userId);

    /// <summary>
    /// Available listings scored against a wanted item, best first. Returns null when the wanted item does not exist.
    /// </summary>
    Task<List<WantedItemMatchDto>?> GetMatchesAsync(Guid id);
}
