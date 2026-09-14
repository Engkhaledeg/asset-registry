using AssetRegistry.Application.Abstractions;
using AssetRegistry.Application.Assets.Dtos;
using AssetRegistry.Application.Assets.Mapping;
using AssetRegistry.Application.Common;
using AssetRegistry.Application.Exceptions;
using AssetRegistry.Domain.Entities;

namespace AssetRegistry.Application.Assets;

public sealed class AssetService : IAssetService
{
    private readonly IAssetRepository _assets;
    private readonly ILocationRepository _locations;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public AssetService(
        IAssetRepository assets,
        ILocationRepository locations,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _assets = assets;
        _locations = locations;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<PagedResult<AssetDto>> SearchAsync(AssetSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var result = await _assets.SearchAsync(criteria, cancellationToken);

        return new PagedResult<AssetDto>(
            result.Assets.ToDtos(),
            result.TotalCount,
            criteria.Page,
            criteria.PageSize);
    }

    public async Task<AssetDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var asset = await LoadAssetAsync(id, cancellationToken);
        return asset.ToDto();
    }

    public async Task<AssetDto> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken)
    {
        await GuardAssetTagIsFreeAsync(request.AssetTag, excludedAssetId: null, cancellationToken);
        await GuardLocationExistsAsync(request.LocationId, cancellationToken);

        var asset = new Asset(
            request.AssetTag,
            request.Name,
            request.Manufacturer,
            request.SerialNumber,
            request.PurchasedOn,
            request.PurchasePrice,
            request.LocationId,
            _clock.Today);

        _assets.Add(asset);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(asset.Id, cancellationToken);
    }

    public async Task<AssetDto> UpdateAsync(int id, UpdateAssetRequest request, CancellationToken cancellationToken)
    {
        var asset = await LoadAssetAsync(id, cancellationToken);
        await GuardLocationExistsAsync(request.LocationId, cancellationToken);

        asset.UpdateDetails(
            request.Name,
            request.Manufacturer,
            request.SerialNumber,
            request.PurchasedOn,
            request.PurchasePrice,
            _clock.Today);

        asset.MoveTo(request.LocationId);
        asset.ChangeStatus(request.Status);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(asset.Id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var asset = await LoadAssetAsync(id, cancellationToken);

        _assets.Remove(asset);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Asset> LoadAssetAsync(int id, CancellationToken cancellationToken)
    {
        return await _assets.FindByIdAsync(id, cancellationToken)
               ?? throw new NotFoundException("Asset", id);
    }

    private async Task GuardAssetTagIsFreeAsync(string assetTag, int? excludedAssetId, CancellationToken cancellationToken)
    {
        if (await _assets.AssetTagIsTakenAsync(assetTag, excludedAssetId, cancellationToken))
        {
            throw new ConflictException($"Asset tag '{assetTag}' is already in use.");
        }
    }

    private async Task GuardLocationExistsAsync(int locationId, CancellationToken cancellationToken)
    {
        if (!await _locations.ExistsAsync(locationId, cancellationToken))
        {
            throw new NotFoundException("Location", locationId);
        }
    }
}
