using AssetRegistry.Domain.Validation;

namespace AssetRegistry.Domain.Entities;

public class Location
{
    private readonly List<Asset> _assets = new();

    private Location()
    {
    }

    public Location(string name, string building)
    {
        Name = Ensure.MaxLength(Ensure.NotBlank(name, nameof(name)), 100, nameof(name));
        Building = Ensure.MaxLength(Ensure.NotBlank(building, nameof(building)), 100, nameof(building));
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Building { get; private set; } = string.Empty;

    public IReadOnlyCollection<Asset> Assets => _assets;
}
