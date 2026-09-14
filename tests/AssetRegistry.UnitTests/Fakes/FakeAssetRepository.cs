using AssetRegistry.Application.Abstractions;
using AssetRegistry.Application.Assets;
using AssetRegistry.Domain.Entities;

namespace AssetRegistry.UnitTests.Fakes;

public class FakeAssetRepository : IAssetRepository
{
    private readonly List<Asset> _assets = new();
    private int _nextId = 1;

    public IReadOnlyList<Asset> Assets => _assets;

    public Task<Asset?> FindByIdAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_assets.FirstOrDefault(asset => asset.Id == id));
    }

    public Task<AssetSearchResult> SearchAsync(AssetSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var page = _assets.Skip(criteria.Skip).Take(criteria.PageSize).ToList();
        return Task.FromResult(new AssetSearchResult(page, _assets.Count));
    }

    public Task<bool> AssetTagIsTakenAsync(string assetTag, int? excludedAssetId, CancellationToken cancellationToken)
    {
        var normalisedTag = assetTag.Trim().ToUpperInvariant();

        return Task.FromResult(_assets.Any(asset =>
            asset.AssetTag == normalisedTag && asset.Id != excludedAssetId));
    }

    public void Add(Asset asset)
    {
        AssignIdentity(asset);
        _assets.Add(asset);
    }

    public void Remove(Asset asset)
    {
        _assets.Remove(asset);
    }

    private void AssignIdentity(Asset asset)
    {
        typeof(Asset).GetProperty(nameof(Asset.Id))!.SetValue(asset, _nextId++);
    }
}
