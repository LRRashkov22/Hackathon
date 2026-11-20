using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Passalong.Core.Entities;
using Passalong.Core.Enums;

namespace Passalong.Infrastructure.Persistence;
public static class DatabaseSeeder
{
    public static async Task SeedAsync(PassalongDbContext db, UserManager<ApplicationUser> users)
    {
        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(new[] { "Technology", "Home & Furniture", "Books", "Sports", "Kitchen", "Clothing" }.Select(x => new Category { Name = x }));
            await db.SaveChangesAsync();
        }
        if (await users.Users.AnyAsync()) return;
        var people = new[] { ("maria@passalong.local", "Maria Ivanova", "Sofia"), ("alex@passalong.local", "Alex Petrov", "Plovdiv"), ("elena@passalong.local", "Elena Georgieva", "Sofia"), ("demo@passalong.local", "Demo Member", "Varna") };
        foreach (var p in people) await users.CreateAsync(new ApplicationUser { UserName=p.Item1, Email=p.Item1, DisplayName=p.Item2, Location=p.Item3, ReputationScore=12 }, "Passalong123!");
        var all = await users.Users.ToListAsync(); var categories = await db.Categories.ToListAsync();
        var examples = new[] { ("Dell 24-inch monitor", "A reliable monitor looking for its next desk.", 0, 0, 1), ("Wooden bookshelf", "Solid pine bookshelf, great condition.", 1, 1, 1), ("The Midnight Library", "A thoughtful novel ready for a new reader.", 2, 0, 2), ("City bicycle", "Well-maintained commuter bicycle.", 3, 1, 0), ("Kitchen mixer", "Compact mixer with all attachments.", 4, 0, 3), ("Wireless headphones", "Comfortable headphones, fully working.", 0, 1, 2) };
        foreach (var x in examples) db.Listings.Add(new Listing { OwnerId=all[x.Item4 % all.Count].Id, CategoryId=categories[x.Item3].Id, Title=x.Item1, Description=x.Item2, Condition=ListingCondition.Good, ExchangeType=x.Item4 % 2 == 0 ? ExchangeType.GiveAway : ExchangeType.Swap, Location=all[x.Item4 % all.Count].Location!, Images = [new ListingImage { ImageUrl="https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=900&q=80" }] });
        db.WantedItems.Add(new WantedItem { UserId=all[0].Id, CategoryId=categories[0].Id, Title="A monitor for remote work", Description="Looking for a working monitor.", PreferredCondition=ListingCondition.Good, Location="Sofia" });
        await db.SaveChangesAsync();
    }
}
