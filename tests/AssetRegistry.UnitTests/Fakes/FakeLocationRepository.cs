using AssetRegistry.Application.Abstractions;
using AssetRegistry.Domain.Entities;

namespace AssetRegistry.UnitTests.Fakes;

public class FakeLocationRepository : ILocationRepository
{
    private readonly HashSet<int> _existingIds;

    public FakeLocationRepository(params int[] existingIds)
    {
        _existingIds = existingIds.ToHashSet();
    }

    public Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_existingIds.Contains(id));
    }
}
