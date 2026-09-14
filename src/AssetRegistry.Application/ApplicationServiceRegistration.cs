using AssetRegistry.Application.Assets;
using AssetRegistry.Application.Locations;
using Microsoft.Extensions.DependencyInjection;

namespace AssetRegistry.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAssetService, AssetService>();
        services.AddScoped<ILocationService, LocationService>();

        return services;
    }
}
