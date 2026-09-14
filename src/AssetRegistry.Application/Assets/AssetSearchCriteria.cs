using AssetRegistry.Domain.Enums;

namespace AssetRegistry.Application.Assets;

public sealed record AssetSearchCriteria(
    string? SearchTerm,
    AssetStatus? Status,
    int? LocationId,
    AssetSortField SortBy,
    bool SortDescending,
    int Page,
    int PageSize)
{
    private const int MaxPageSize = 100;

    public int Skip => (Page - 1) * PageSize;

    public static AssetSearchCriteria Create(
        string? searchTerm,
        AssetStatus? status,
        int? locationId,
        AssetSortField sortBy,
        bool sortDescending,
        int page,
        int pageSize)
    {
        return new AssetSearchCriteria(
            string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim(),
            status,
            locationId,
            sortBy,
            sortDescending,
            Math.Max(page, 1),
            Math.Clamp(pageSize, 1, MaxPageSize));
    }
}
