using Microsoft.EntityFrameworkCore;
using Passalong.Application.Abstractions.Persistence;
using Passalong.Application.Common;
using Passalong.Application.DTOs.Listings;
using Passalong.Application.Services.Contracts;
using Passalong.Core.Entities;
using Passalong.Core.Enums;

namespace Passalong.Application.Services.Implementation;

public class ListingService(IApplicationDbContext db) : IListingService
{
    public async Task<ListingPageDto> BrowseAsync(BrowseListingsQuery query, Guid? currentUserId)
    {
        var q = db.Listings.Where(x => x.Status == ListingStatus.Available);

        if (currentUserId is Guid excludedOwnerId)
        {
            q = q.Where(x => x.OwnerId != excludedOwnerId);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            // LIKE is case-insensitive for ASCII text on SQLite.
            q = q.Where(x => EF.Functions.Like(x.Title, $"%{query.Search}%"));
        }

        if (query.CategoryId.HasValue)
        {
            q = q.Where(x => x.CategoryId == query.CategoryId);
        }

        if (query.ExchangeType.HasValue)
        {
            q = q.Where(x => x.ExchangeType == query.ExchangeType);
        }

        if (query.Condition.HasValue)
        {
            q = q.Where(x => x.Condition == query.Condition);
        }

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new ListingSummaryDto(
                x.Id,
                x.Title,
                x.Description,
                x.Condition,
                x.ExchangeType,
                x.Location,
                x.CreatedAt,
                x.Category.Name,
                new ListingOwnerDto(x.Owner.Id, x.Owner.DisplayName),
                x.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl)))
            .ToListAsync();

        return new ListingPageDto(total, query.Page, query.PageSize, items);
    }

    public Task<Listing?> GetAsync(Guid id)
    {
        return db.Listings
            .Include(x => x.Images)
            .Include(x => x.Category)
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Listing>> GetMineAsync(Guid userId)
    {
        return db.Listings
            .Include(x => x.Images)
            .Include(x => x.Category)
            .Where(x => x.OwnerId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Result<ListingCreatedDto>> CreateAsync(Guid userId, CreateListingRequest request)
    {
        if (!await db.Categories.AnyAsync(c => c.Id == request.CategoryId))
        {
            return Error.Validation("Unknown category.");
        }

        if (request.ExchangeType == ExchangeType.Swap && string.IsNullOrWhiteSpace(request.DesiredSwap))
        {
            return Error.Validation("Describe what you want in return for a swap.");
        }

        var listing = new Listing
        {
            OwnerId = userId,
            CategoryId = request.CategoryId,
            Title = request.Title,
            Description = request.Description,
            Condition = request.Condition,
            ExchangeType = request.ExchangeType,
            DesiredSwap = request.DesiredSwap,
            Location = request.Location,
            Images = (request.ImageUrls ?? [])
                .Select((url, i) => new ListingImage { ImageUrl = url, DisplayOrder = i })
                .ToList()
        };

        db.Listings.Add(listing);
        await db.SaveChangesAsync();

        return new ListingCreatedDto(
            listing.Id,
            listing.Title,
            listing.Description,
            listing.Condition,
            listing.ExchangeType,
            listing.DesiredSwap,
            listing.Location,
            listing.Status,
            listing.CreatedAt,
            listing.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList());
    }
}
