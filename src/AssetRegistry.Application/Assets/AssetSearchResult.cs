using AssetRegistry.Domain.Entities;

namespace AssetRegistry.Application.Assets;

public sealed record AssetSearchResult(IReadOnlyList<Asset> Assets, int TotalCount);
