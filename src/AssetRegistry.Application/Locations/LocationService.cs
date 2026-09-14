using AssetRegistry.Application.Abstractions;
using AssetRegistry.Application.Locations.Dtos;

namespace AssetRegistry.Application.Locations;

public sealed class LocationService : ILocationService
{
    private readonly ILocationRepository _locations;

    public LocationService(ILocationRepository locations)
    {
        _locations = locations;
    }

    public async Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var locations = await _locations.GetAllAsync(cancellationToken);

        return locations
            .Select(location => new LocationDto(location.Id, location.Name, location.Building))
            .ToList();
    }
}
