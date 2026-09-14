using AssetRegistry.Domain.Entities;

namespace AssetRegistry.Application.Abstractions;

public interface ILocationRepository
{
    Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
}
