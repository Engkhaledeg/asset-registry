using AssetRegistry.Infrastructure.Persistence;

namespace AssetRegistry.Api.Configuration;

public static class DatabaseInitialisationExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<DatabaseInitialiser>();

        await initialiser.MigrateAndSeedAsync(cancellationToken);
    }
}
