using Microsoft.EntityFrameworkCore;
using Passalong.Application.Abstractions.Persistence;
using Passalong.Application.Common;
using Passalong.Application.DTOs.ExchangeRequests;
using Passalong.Application.Services.Contracts;
using Passalong.Core.Entities;
using Passalong.Core.Enums;

namespace Passalong.Application.Services.Implementation;

public class ExchangeRequestService(IApplicationDbContext db) : IExchangeRequestService
{
    public async Task<Result<ExchangeRequest>> CreateAsync(Guid userId, CreateExchangeRequest request)
    {
        var listing = await db.Listings.FirstOrDefaultAsync(x => x.Id == request.ListingId);

        if (listing is null || listing.Status != ListingStatus.Available)
        {
            return Error.Validation("This item is no longer available.");
        }

        if (listing.OwnerId == userId)
        {
            return Error.Validation("You cannot request your own listing.");
        }

        var alreadyPending = await db.ExchangeRequests.AnyAsync(x =>
            x.ListingId == request.ListingId
            && x.RequesterId == userId
            && x.Status == ExchangeRequestStatus.Pending);

        if (alreadyPending)
        {
            return Error.Conflict("You already have a pending request.");
        }

        if (listing.ExchangeType == ExchangeType.Swap)
        {
            if (request.OfferedListingId is not Guid offeredId)
            {
                return Error.Validation("A swap needs an offered item.");
            }

            var offered = await db.Listings.FindAsync(offeredId);

            if (offered is null || offered.OwnerId != userId || offered.Status != ListingStatus.Available)
            {
                return Error.Validation("The offered item must be your available listing.");
            }
        }

        var entity = new ExchangeRequest
        {
            ListingId = listing.Id,
            RequesterId = userId,
            OwnerId = listing.OwnerId,
            OfferedListingId = request.OfferedListingId,
            Message = request.Message
        };

        db.ExchangeRequests.Add(entity);
        await db.SaveChangesAsync();

        return entity;
    }

    public Task<List<ExchangeRequest>> GetIncomingAsync(Guid userId)
    {
        return db.ExchangeRequests
            .Include(x => x.Listing)
            .Include(x => x.Requester)
            .Where(x => x.OwnerId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public Task<List<ExchangeRequest>> GetOutgoingAsync(Guid userId)
    {
        return db.ExchangeRequests
            .Include(x => x.Listing)
            .Include(x => x.Owner)
            .Where(x => x.RequesterId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Result<ExchangeRequest>> AcceptAsync(Guid userId, Guid id)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        var request = await FindWithListingAsync(id);

        if (request is null)
        {
            return Error.NotFound();
        }

        if (request.OwnerId != userId || request.Status != ExchangeRequestStatus.Pending)
        {
            return Error.Validation("This request cannot be accepted.");
        }

        if (request.Listing.Status != ListingStatus.Available)
        {
            return Error.Conflict("Listing is not available.");
        }

        request.Status = ExchangeRequestStatus.Accepted;
        request.Listing.Status = ListingStatus.Reserved;
        request.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return request;
    }

    public Task<Result<ExchangeRequest>> RejectAsync(Guid userId, Guid id)
    {
        return ChangeStatusAsync(id, ExchangeRequestStatus.Rejected, x => x.OwnerId == userId);
    }

    public Task<Result<ExchangeRequest>> CancelAsync(Guid userId, Guid id)
    {
        return ChangeStatusAsync(id, ExchangeRequestStatus.Cancelled, x => x.RequesterId == userId);
    }

    public async Task<Result<HandoverConfirmationDto>> ConfirmHandoverAsync(Guid userId, Guid id)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        var request = await FindWithListingAsync(id);

        if (request is null)
        {
            return Error.NotFound();
        }

        if (request.Status != ExchangeRequestStatus.Accepted
            || (request.RequesterId != userId && request.OwnerId != userId))
        {
            return Error.Validation("This handover cannot be confirmed.");
        }

        if (request.RequesterId == userId)
        {
            request.RequesterConfirmed = true;
        }
        else
        {
            request.OwnerConfirmed = true;
        }

        if (request.RequesterConfirmed && request.OwnerConfirmed)
        {
            request.Listing.OwnerId = request.RequesterId;
            request.Listing.Status = ListingStatus.Transferred;
            request.Status = ExchangeRequestStatus.Completed;

            db.Transfers.Add(new Transfer
            {
                ListingId = request.ListingId,
                PreviousOwnerId = request.OwnerId,
                NewOwnerId = request.RequesterId,
                ExchangeRequestId = request.Id
            });

            if (request.OfferedListingId is Guid offerId)
            {
                var offered = await db.Listings.FindAsync(offerId);

                if (offered is null
                    || offered.OwnerId != request.RequesterId
                    || offered.Status != ListingStatus.Available)
                {
                    return Error.Validation("The offered item is no longer available.");
                }

                offered.OwnerId = request.OwnerId;
                offered.Status = ListingStatus.Transferred;

                db.Transfers.Add(new Transfer
                {
                    ListingId = offerId,
                    PreviousOwnerId = request.RequesterId,
                    NewOwnerId = request.OwnerId,
                    ExchangeRequestId = request.Id
                });
            }
        }

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return new HandoverConfirmationDto(
            request.Id,
            request.Status,
            request.RequesterConfirmed,
            request.OwnerConfirmed);
    }

    private Task<ExchangeRequest?> FindWithListingAsync(Guid id)
    {
        return db.ExchangeRequests
            .Include(x => x.Listing)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    private async Task<Result<ExchangeRequest>> ChangeStatusAsync(
        Guid id,
        ExchangeRequestStatus status,
        Func<ExchangeRequest, bool> permitted)
    {
        var request = await db.ExchangeRequests.FindAsync(id);

        if (request is null)
        {
            return Error.NotFound();
        }

        if (!permitted(request) || request.Status != ExchangeRequestStatus.Pending)
        {
            return Error.Validation("This request cannot be changed.");
        }

        request.Status = status;
        request.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return request;
    }
}
