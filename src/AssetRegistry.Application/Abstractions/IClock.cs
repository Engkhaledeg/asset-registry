namespace AssetRegistry.Application.Abstractions;

public interface IClock
{
    DateOnly Today { get; }
}
