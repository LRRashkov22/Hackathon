using Microsoft.EntityFrameworkCore;
using Passalong.Application.Abstractions.Persistence;
using Passalong.Application.DTOs.WantedItems;
using Passalong.Application.Services.Contracts;
using Passalong.Core.Entities;
using Passalong.Core.Enums;

namespace Passalong.Application.Services.Implementation;

public class WantedItemService(IApplicationDbContext db) : IWantedItemService
{
    private const int CategoryScore = 50;

    private const int TitleScore = 25;

    private const int ConditionScore = 15;

    private const int LocationScore = 10;

    public async Task<WantedItem> CreateAsync(Guid userId, CreateWantedItemRequest request)
    {
        var item = new WantedItem
        {
            UserId = userId,
            CategoryId = request.CategoryId,
            Title = request.Title,
            Description = request.Description,
            PreferredCondition = request.PreferredCondition,
            Location = request.Location
        };

        db.WantedItems.Add(item);
        await db.SaveChangesAsync();

        return item;
    }

    public Task<List<WantedItem>> GetMineAsync(Guid userId)
    {
        return db.WantedItems
            .Include(x => x.Category)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<WantedItemMatchDto>?> GetMatchesAsync(Guid id)
    {
        var wanted = await db.WantedItems.FindAsync(id);

        if (wanted is null)
        {
            return null;
        }

        var words = wanted.Title.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var available = await db.Listings
            .Include(x => x.Category)
            .Include(x => x.Owner)
            .Where(x => x.Status == ListingStatus.Available)
            .ToListAsync();

        return available
            .Select(x => new
            {
                listing = x,
                score = Score(wanted, words, x)
            })
            .Where(x => x.score > 0)
            .OrderByDescending(x => x.score)
            .Select(x => new WantedItemMatchDto(
                x.score,
                new MatchedListingDto(
                    x.listing.Id,
                    x.listing.Title,
                    x.listing.Description,
                    x.listing.Location,
                    x.listing.Condition,
                    x.listing.ExchangeType,
                    x.listing.Category.Name,
                    x.listing.Owner.DisplayName)))
            .ToList();
    }

    private static int Score(WantedItem wanted, string[] words, Listing listing)
    {
        return (listing.CategoryId == wanted.CategoryId ? CategoryScore : 0)
            + (words.Any(k => listing.Title.ToLower().Contains(k)) ? TitleScore : 0)
            + (wanted.PreferredCondition == listing.Condition ? ConditionScore : 0)
            + (string.Equals(wanted.Location, listing.Location, StringComparison.OrdinalIgnoreCase) ? LocationScore : 0);
    }
}
