using Microsoft.Extensions.DependencyInjection;
using Passalong.Application.Services.Contracts;
using Passalong.Application.Services.Implementation;

namespace Passalong.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICommunityService, CommunityService>();
        services.AddScoped<IExchangeRequestService, ExchangeRequestService>();
        services.AddScoped<IListingService, ListingService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWantedItemService, WantedItemService>();

        return services;
    }
}
