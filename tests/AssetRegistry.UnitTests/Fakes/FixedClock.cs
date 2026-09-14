using AssetRegistry.Application.Abstractions;

namespace AssetRegistry.UnitTests.Fakes;

public class FixedClock : IClock
{
    public FixedClock(DateOnly today)
    {
        Today = today;
    }

    public DateOnly Today { get; }
}
