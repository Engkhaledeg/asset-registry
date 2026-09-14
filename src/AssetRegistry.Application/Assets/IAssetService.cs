using AssetRegistry.Application.Assets.Dtos;
using AssetRegistry.Application.Common;

namespace AssetRegistry.Application.Assets;

public interface IAssetService
{
    Task<PagedResult<AssetDto>> SearchAsync(AssetSearchCriteria criteria, CancellationToken cancellationToken);

    Task<AssetDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<AssetDto> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken);

    Task<AssetDto> UpdateAsync(int id, UpdateAssetRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
