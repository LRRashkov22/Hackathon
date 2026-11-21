using Passalong.Application.DTOs.Community;

namespace Passalong.Application.Services.Contracts;

public interface ICommunityService
{
    Task<ImpactSummaryDto> GetImpactSummaryAsync();

    Task<List<LeaderboardEntryDto>> GetLeaderboardAsync();

    Task<List<TransferHistoryDto>> GetItemHistoryAsync(Guid listingId);
}
