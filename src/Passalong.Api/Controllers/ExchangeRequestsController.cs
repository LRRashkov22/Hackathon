using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passalong.Api.Contracts;
using Passalong.Core.Entities;
using Passalong.Core.Enums;
using Passalong.Infrastructure.Persistence;
namespace Passalong.Api.Controllers;
[Authorize, ApiController, Route("api/exchange-requests")]
public class ExchangeRequestsController(PassalongDbContext db) : ControllerBase
{
    Guid Me => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpPost] public async Task<IActionResult> Create(CreateExchangeRequest request)
    {
        var listing=await db.Listings.FirstOrDefaultAsync(x=>x.Id==request.ListingId); if(listing is null || listing.Status!=ListingStatus.Available) return BadRequest("This item is no longer available."); if(listing.OwnerId==Me)return BadRequest("You cannot request your own listing.");
        if(await db.ExchangeRequests.AnyAsync(x=>x.ListingId==request.ListingId&&x.RequesterId==Me&&x.Status==ExchangeRequestStatus.Pending))return Conflict("You already have a pending request.");
        if(listing.ExchangeType==ExchangeType.Swap) { if(request.OfferedListingId is null) return BadRequest("A swap needs an offered item."); var offered=await db.Listings.FindAsync(request.OfferedListingId); if(offered is null||offered.OwnerId!=Me||offered.Status!=ListingStatus.Available)return BadRequest("The offered item must be your available listing."); }
        var entity=new ExchangeRequest { ListingId=listing.Id,RequesterId=Me,OwnerId=listing.OwnerId,OfferedListingId=request.OfferedListingId,Message=request.Message }; db.ExchangeRequests.Add(entity);await db.SaveChangesAsync();return Created($"api/exchange-requests/{entity.Id}",entity);
    }
    [HttpGet("incoming")] public Task<List<ExchangeRequest>> Incoming()=>db.ExchangeRequests.Include(x=>x.Listing).Include(x=>x.Requester).Where(x=>x.OwnerId==Me).OrderByDescending(x=>x.CreatedAt).ToListAsync();
    [HttpGet("outgoing")] public Task<List<ExchangeRequest>> Outgoing()=>db.ExchangeRequests.Include(x=>x.Listing).Include(x=>x.Owner).Where(x=>x.RequesterId==Me).OrderByDescending(x=>x.CreatedAt).ToListAsync();
    [HttpPost("{id:guid}/accept")] public async Task<IActionResult> Accept(Guid id) { await using var tx=await db.Database.BeginTransactionAsync(); var r=await db.ExchangeRequests.Include(x=>x.Listing).FirstOrDefaultAsync(x=>x.Id==id); if(r is null)return NotFound(); if(r.OwnerId!=Me||r.Status!=ExchangeRequestStatus.Pending)return BadRequest("This request cannot be accepted."); if(r.Listing.Status!=ListingStatus.Available)return Conflict("Listing is not available."); r.Status=ExchangeRequestStatus.Accepted;r.Listing.Status=ListingStatus.Reserved;r.UpdatedAt=DateTime.UtcNow; await db.SaveChangesAsync();await tx.CommitAsync();return Ok(r); }
    [HttpPost("{id:guid}/reject")] public async Task<IActionResult> Reject(Guid id)=>await Change(id,ExchangeRequestStatus.Rejected, x=>x.OwnerId==Me);
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id)=>await Change(id,ExchangeRequestStatus.Cancelled, x=>x.RequesterId==Me);
    async Task<IActionResult> Change(Guid id,ExchangeRequestStatus status,Func<ExchangeRequest,bool> permitted) { var r=await db.ExchangeRequests.FindAsync(id);if(r is null)return NotFound();if(!permitted(r)||r.Status!=ExchangeRequestStatus.Pending)return BadRequest("This request cannot be changed.");r.Status=status;r.UpdatedAt=DateTime.UtcNow;await db.SaveChangesAsync();return Ok(r); }
    [HttpPost("{id:guid}/confirm-handover")] public async Task<IActionResult> Confirm(Guid id)
    { await using var tx=await db.Database.BeginTransactionAsync();var r=await db.ExchangeRequests.Include(x=>x.Listing).FirstOrDefaultAsync(x=>x.Id==id);if(r is null)return NotFound();if(r.Status!=ExchangeRequestStatus.Accepted||(r.RequesterId!=Me&&r.OwnerId!=Me))return BadRequest("This handover cannot be confirmed.");if(r.RequesterId==Me)r.RequesterConfirmed=true;else r.OwnerConfirmed=true;if(r.RequesterConfirmed&&r.OwnerConfirmed){r.Listing.OwnerId=r.RequesterId;r.Listing.Status=ListingStatus.Transferred;r.Status=ExchangeRequestStatus.Completed;db.Transfers.Add(new Transfer{ListingId=r.ListingId,PreviousOwnerId=r.OwnerId,NewOwnerId=r.RequesterId,ExchangeRequestId=r.Id});if(r.OfferedListingId is Guid offerId){var offered=await db.Listings.FindAsync(offerId);if(offered is null||offered.OwnerId!=r.RequesterId||offered.Status!=ListingStatus.Available)return BadRequest("The offered item is no longer available.");offered.OwnerId=r.OwnerId;offered.Status=ListingStatus.Transferred;db.Transfers.Add(new Transfer{ListingId=offerId,PreviousOwnerId=r.RequesterId,NewOwnerId=r.OwnerId,ExchangeRequestId=r.Id});}}await db.SaveChangesAsync();await tx.CommitAsync();return Ok(new{r.Id,r.Status,r.RequesterConfirmed,r.OwnerConfirmed}); }
}
