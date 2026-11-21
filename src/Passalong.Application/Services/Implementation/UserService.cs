using Microsoft.EntityFrameworkCore;
using Passalong.Application.Abstractions.Persistence;
using Passalong.Application.DTOs.Users;
using Passalong.Application.Services.Contracts;

namespace Passalong.Application.Services.Implementation;

public class UserService(IApplicationDbContext db) : IUserService
{
    public async Task<UserProfileDto?> GetProfileAsync(Guid id)
    {
        var user = await db.Users.FindAsync(id);

        if (user is null)
        {
            return null;
        }

        var ownedListings = await db.Listings
            .Where(x => x.OwnerId == id)
            .Select(x => new UserListingDto(x.Id, x.Title, x.Status))
            .ToListAsync();

        var receivedReviews = await db.Reviews
            .Where(x => x.ReviewedUserId == id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new UserReviewDto(x.Rating, x.Comment, x.CreatedAt))
            .ToListAsync();

        return new UserProfileDto(
            user.Id,
            user.DisplayName,
            user.Bio,
            user.Location,
            user.AvatarUrl,
            user.ReputationScore,
            ownedListings,
            receivedReviews);
    }
}
