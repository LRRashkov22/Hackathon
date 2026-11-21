using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Passalong.Core.Entities;

namespace Passalong.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }

    DbSet<Listing> Listings { get; }

    DbSet<ListingImage> ListingImages { get; }

    DbSet<WantedItem> WantedItems { get; }

    DbSet<ExchangeRequest> ExchangeRequests { get; }

    DbSet<Transfer> Transfers { get; }

    DbSet<Review> Reviews { get; }

    DbSet<ApplicationUser> Users { get; }

    /// <summary>
    /// Database facade, used for explicit transactions.
    /// </summary>
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
