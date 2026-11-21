using Passalong.Application.Common;
using Passalong.Application.DTOs.Listings;
using Passalong.Core.Entities;

namespace Passalong.Application.Services.Contracts;

public interface IListingService
{
    /// <summary>
    /// Available listings matching the query, excluding the current user's own listings when signed in.
    /// </summary>
    Task<ListingPageDto> BrowseAsync(BrowseListingsQuery query, Guid? currentUserId);

    Task<Listing?> GetAsync(Guid id);

    Task<List<Listing>> GetMineAsync(Guid userId);

    Task<Result<ListingCreatedDto>> CreateAsync(Guid userId, CreateListingRequest request);
}
