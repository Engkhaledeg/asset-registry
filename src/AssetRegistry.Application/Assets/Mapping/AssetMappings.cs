using AssetRegistry.Application.Assets.Dtos;
using AssetRegistry.Domain.Entities;

namespace AssetRegistry.Application.Assets.Mapping;

public static class AssetMappings
{
    public static AssetDto ToDto(this Asset asset)
    {
        return new AssetDto(
            asset.Id,
            asset.AssetTag,
            asset.Name,
            asset.Manufacturer,
            asset.SerialNumber,
            asset.PurchasedOn,
            asset.PurchasePrice,
            asset.Status,
            asset.LocationId,
            asset.Location?.Name ?? string.Empty);
    }

    public static IReadOnlyList<AssetDto> ToDtos(this IEnumerable<Asset> assets)
    {
        return assets.Select(ToDto).ToList();
    }
}
