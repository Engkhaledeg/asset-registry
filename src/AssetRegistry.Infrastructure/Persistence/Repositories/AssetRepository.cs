using AssetRegistry.Application.Abstractions;
using AssetRegistry.Application.Assets;
using AssetRegistry.Domain.Entities;
using AssetRegistry.Infrastructure.Persistence.Queries;
using Microsoft.EntityFrameworkCore;

namespace AssetRegistry.Infrastructure.Persistence.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly AssetRegistryDbContext _context;

    public AssetRepository(AssetRegistryDbContext context)
    {
        _context = context;
    }

    public async Task<Asset?> FindByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Assets
            .Include(asset => asset.Location)
            .FirstOrDefaultAsync(asset => asset.Id == id, cancellationToken);
    }

    public async Task<AssetSearchResult> SearchAsync(AssetSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var filtered = _context.Assets
            .AsNoTracking()
            .Include(asset => asset.Location)
            .ApplyFilters(criteria);

        var totalCount = await filtered.CountAsync(cancellationToken);

        var assets = await filtered
            .ApplySorting(criteria)
            .ApplyPaging(criteria)
            .ToListAsync(cancellationToken);

        return new AssetSearchResult(assets, totalCount);
    }

    public async Task<bool> AssetTagIsTakenAsync(string assetTag, int? excludedAssetId, CancellationToken cancellationToken)
    {
        var normalisedTag = assetTag.Trim().ToUpperInvariant();

        return await _context.Assets
            .AsNoTracking()
            .AnyAsync(
                asset => asset.AssetTag == normalisedTag && (excludedAssetId == null || asset.Id != excludedAssetId),
                cancellationToken);
    }

    public void Add(Asset asset)
    {
        _context.Assets.Add(asset);
    }

    public void Remove(Asset asset)
    {
        _context.Assets.Remove(asset);
    }
}
