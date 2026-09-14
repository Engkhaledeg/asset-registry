using AssetRegistry.Application.Abstractions;
using AssetRegistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetRegistry.Infrastructure.Persistence.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly AssetRegistryDbContext _context;

    public LocationRepository(AssetRegistryDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Locations
            .AsNoTracking()
            .OrderBy(location => location.Building)
            .ThenBy(location => location.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Locations.AnyAsync(location => location.Id == id, cancellationToken);
    }
}
