using AssetRegistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetRegistry.Infrastructure.Persistence;

public class DatabaseInitialiser
{
    private readonly AssetRegistryDbContext _context;

    public DatabaseInitialiser(AssetRegistryDbContext context)
    {
        _context = context;
    }

    public async Task MigrateAndSeedAsync(CancellationToken cancellationToken)
    {
        await _context.Database.MigrateAsync(cancellationToken);

        if (await _context.Locations.AnyAsync(cancellationToken))
        {
            return;
        }

        _context.Locations.AddRange(
            new Location("Ground floor workshop", "Riverside"),
            new Location("Second floor office", "Riverside"),
            new Location("Server room", "Northgate"),
            new Location("Remote worker", "Off site"));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
