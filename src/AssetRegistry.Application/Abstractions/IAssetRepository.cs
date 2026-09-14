using AssetRegistry.Application.Assets;
using AssetRegistry.Domain.Entities;

namespace AssetRegistry.Application.Abstractions;

public interface IAssetRepository
{
    Task<Asset?> FindByIdAsync(int id, CancellationToken cancellationToken);

    Task<AssetSearchResult> SearchAsync(AssetSearchCriteria criteria, CancellationToken cancellationToken);

    Task<bool> AssetTagIsTakenAsync(string assetTag, int? excludedAssetId, CancellationToken cancellationToken);

    void Add(Asset asset);

    void Remove(Asset asset);
}
