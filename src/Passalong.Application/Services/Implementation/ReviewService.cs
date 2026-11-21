using Microsoft.EntityFrameworkCore;
using Passalong.Application.Abstractions.Persistence;
using Passalong.Application.Common;
using Passalong.Application.DTOs.Reviews;
using Passalong.Application.Services.Contracts;
using Passalong.Core.Entities;

namespace Passalong.Application.Services.Implementation;

public class ReviewService(IApplicationDbContext db) : IReviewService
{
    public async Task<Result> CreateAsync(Guid userId, CreateReviewRequest request)
    {
        if (request.Rating is < 1 or > 5)
        {
            return Error.Validation("Rating must be from 1 to 5.");
        }

        var transfer = await db.Transfers.FindAsync(request.TransferId);

        if (transfer is null || (transfer.PreviousOwnerId != userId && transfer.NewOwnerId != userId))
        {
            return Error.Forbidden();
        }

        var other = transfer.PreviousOwnerId == userId ? transfer.NewOwnerId : transfer.PreviousOwnerId;

        if (await db.Reviews.AnyAsync(r => r.TransferId == request.TransferId && r.ReviewerId == userId))
        {
            return Error.Conflict("You already reviewed this transfer.");
        }

        db.Reviews.Add(new Review
        {
            TransferId = request.TransferId,
            ReviewerId = userId,
            ReviewedUserId = other,
            Rating = request.Rating,
            Comment = request.Comment
        });

        var reviewedUser = await db.Users.FindAsync(other);

        if (reviewedUser is not null)
        {
            reviewedUser.ReputationScore += request.Rating;
        }

        await db.SaveChangesAsync();

        return Result.Success();
    }
}
