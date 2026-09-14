using System.ComponentModel.DataAnnotations;
using AssetRegistry.Application.Assets;
using AssetRegistry.Domain.Enums;

namespace AssetRegistry.Api.Contracts;

public sealed class AssetSearchRequest
{
    public string? SearchTerm { get; init; }

    public AssetStatus? Status { get; init; }

    public int? LocationId { get; init; }

    public AssetSortField SortBy { get; init; } = AssetSortField.AssetTag;

    public bool SortDescending { get; init; }

    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    public AssetSearchCriteria ToCriteria()
    {
        return AssetSearchCriteria.Create(SearchTerm, Status, LocationId, SortBy, SortDescending, Page, PageSize);
    }
}
