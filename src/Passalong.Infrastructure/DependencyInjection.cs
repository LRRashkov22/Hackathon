using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Passalong.Application.Abstractions.Identity;
using Passalong.Application.Abstractions.Persistence;
using Passalong.Core.Entities;
using Passalong.Infrastructure.Authentication;
using Passalong.Infrastructure.Identity;
using Passalong.Infrastructure.Persistence;

namespace Passalong.Infrastructure;

public static class DependencyInjection
{
    private const string DefaultConnection = "Data Source=passalong.db";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("DefaultConnection") ?? DefaultConnection;

        // The database provider is chosen here only; swap UseSqlite to change it.
        services.AddDbContext<PassalongDbContext>(o => o.UseSqlite(connection));

        services
            .AddIdentityCore<ApplicationUser>(o =>
            {
                o.Password.RequiredLength = 8;
                o.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<PassalongDbContext>();

        // Same scoped instance as the Identity stores use.
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<PassalongDbContext>());

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }

    /// <summary>
    /// Applies pending migrations and seeds sample data.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var db = provider.GetRequiredService<PassalongDbContext>();

        await db.Database.MigrateAsync();
        await DatabaseSeeder.SeedAsync(db, provider.GetRequiredService<UserManager<ApplicationUser>>());
    }
}
