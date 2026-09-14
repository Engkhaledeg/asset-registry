using AssetRegistry.Application.Locations.Dtos;

namespace AssetRegistry.Application.Locations;

public interface ILocationService
{
    Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken);
}
