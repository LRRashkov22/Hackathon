using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Passalong.Core.Entities;

namespace Passalong.Infrastructure.Persistence;

public class PassalongDbContext(DbContextOptions<PassalongDbContext> options) : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Category> Categories => Set<Category>(); public DbSet<Listing> Listings => Set<Listing>(); public DbSet<ListingImage> ListingImages => Set<ListingImage>(); public DbSet<WantedItem> WantedItems => Set<WantedItem>(); public DbSet<ExchangeRequest> ExchangeRequests => Set<ExchangeRequest>(); public DbSet<Transfer> Transfers => Set<Transfer>(); public DbSet<Review> Reviews => Set<Review>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<Category>().HasIndex(x => x.Name).IsUnique();
        b.Entity<Listing>().HasIndex(x => new { x.Status, x.CategoryId, x.CreatedAt });
        b.Entity<Listing>().HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Listing>().HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<ListingImage>().HasOne(x => x.Listing).WithMany(x => x.Images).HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<WantedItem>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<ExchangeRequest>().HasIndex(x => new { x.ListingId, x.RequesterId, x.Status });
        b.Entity<ExchangeRequest>().HasOne(x => x.Listing).WithMany().HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<ExchangeRequest>().HasOne(x => x.Requester).WithMany().HasForeignKey(x => x.RequesterId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<ExchangeRequest>().HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Review>().HasIndex(x => new { x.TransferId, x.ReviewerId }).IsUnique();
    }
}
