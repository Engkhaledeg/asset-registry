using AssetRegistry.Application.Abstractions;

namespace AssetRegistry.Infrastructure;

public class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
