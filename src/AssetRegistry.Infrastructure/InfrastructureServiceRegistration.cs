using AssetRegistry.Application.Abstractions;
using AssetRegistry.Infrastructure.Persistence;
using AssetRegistry.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AssetRegistry.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AssetRegistry")
                               ?? throw new InvalidOperationException("Connection string 'AssetRegistry' is not configured.");

        services.AddDbContext<AssetRegistryDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AssetRegistryDbContext>());
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<DatabaseInitialiser>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
