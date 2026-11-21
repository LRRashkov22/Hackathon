using Microsoft.EntityFrameworkCore;
using Passalong.Application.Abstractions.Persistence;
using Passalong.Application.DTOs.Community;
using Passalong.Application.Services.Contracts;
using Passalong.Core.Enums;

namespace Passalong.Application.Services.Implementation;

public class CommunityService(IApplicationDbContext db) : ICommunityService
{
    private const int LeaderboardSize = 10;

    public async Task<ImpactSummaryDto> GetImpactSummaryAsync()
    {
        var completedTransfers = await db.Transfers.CountAsync();
        var itemsReused = await db.Transfers
            .Select(x => x.ListingId)
            .Distinct()
            .CountAsync();
        var activeListings = await db.Listings.CountAsync(x => x.Status == ListingStatus.Available);
        var members = await db.Users.CountAsync();

        return new ImpactSummaryDto(completedTransfers, itemsReused, activeListings, members);
    }

    public Task<List<LeaderboardEntryDto>> GetLeaderboardAsync()
    {
        return db.Users
            .OrderByDescending(x => x.ReputationScore)
            .Take(LeaderboardSize)
            .Select(x => new LeaderboardEntryDto(x.Id, x.DisplayName, x.ReputationScore, x.Location))
            .ToListAsync();
    }

    public Task<List<TransferHistoryDto>> GetItemHistoryAsync(Guid listingId)
    {
        return (
            from t in db.Transfers
            join old in db.Users on t.PreviousOwnerId equals old.Id
            join next in db.Users on t.NewOwnerId equals next.Id
            where t.ListingId == listingId
            orderby t.CompletedAt
            select new TransferHistoryDto(t.Id, t.CompletedAt, old.DisplayName, next.DisplayName))
            .ToListAsync();
    }
}
