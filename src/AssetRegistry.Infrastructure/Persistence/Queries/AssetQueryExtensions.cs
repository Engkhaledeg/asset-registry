using AssetRegistry.Application.Assets;
using AssetRegistry.Domain.Entities;

namespace AssetRegistry.Infrastructure.Persistence.Queries;

public static class AssetQueryExtensions
{
    public static IQueryable<Asset> ApplyFilters(this IQueryable<Asset> assets, AssetSearchCriteria criteria)
    {
        if (criteria.Status.HasValue)
        {
            assets = assets.Where(asset => asset.Status == criteria.Status.Value);
        }

        if (criteria.LocationId.HasValue)
        {
            assets = assets.Where(asset => asset.LocationId == criteria.LocationId.Value);
        }

        if (criteria.SearchTerm is not null)
        {
            var term = criteria.SearchTerm;

            assets = assets.Where(asset =>
                asset.AssetTag.Contains(term) ||
                asset.Name.Contains(term) ||
                asset.SerialNumber.Contains(term));
        }

        return assets;
    }

    public static IQueryable<Asset> ApplySorting(this IQueryable<Asset> assets, AssetSearchCriteria criteria)
    {
        return criteria.SortBy switch
        {
            AssetSortField.Name => assets.SortBy(asset => asset.Name, criteria.SortDescending),
            AssetSortField.PurchasedOn => assets.SortBy(asset => asset.PurchasedOn, criteria.SortDescending),
            AssetSortField.PurchasePrice => assets.SortBy(asset => asset.PurchasePrice, criteria.SortDescending),
            AssetSortField.Status => assets.SortBy(asset => asset.Status, criteria.SortDescending),
            _ => assets.SortBy(asset => asset.AssetTag, criteria.SortDescending)
        };
    }

    public static IQueryable<Asset> ApplyPaging(this IQueryable<Asset> assets, AssetSearchCriteria criteria)
    {
        return assets.Skip(criteria.Skip).Take(criteria.PageSize);
    }

    private static IQueryable<Asset> SortBy<TKey>(
        this IQueryable<Asset> assets,
        System.Linq.Expressions.Expression<Func<Asset, TKey>> selector,
        bool descending)
    {
        return descending ? assets.OrderByDescending(selector) : assets.OrderBy(selector);
    }
}
