using AssetRegistry.Domain.Enums;
using AssetRegistry.Domain.Exceptions;
using AssetRegistry.Domain.Validation;

namespace AssetRegistry.Domain.Entities;

public class Asset
{
    private Asset()
    {
    }

    public Asset(
        string assetTag,
        string name,
        string manufacturer,
        string serialNumber,
        DateOnly purchasedOn,
        decimal purchasePrice,
        int locationId,
        DateOnly today)
    {
        AssetTag = NormaliseTag(assetTag);
        LocationId = locationId;
        Status = AssetStatus.InStock;
        ApplyDetails(name, manufacturer, serialNumber, purchasedOn, purchasePrice, today);
    }

    public int Id { get; private set; }

    public string AssetTag { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string Manufacturer { get; private set; } = string.Empty;

    public string SerialNumber { get; private set; } = string.Empty;

    public DateOnly PurchasedOn { get; private set; }

    public decimal PurchasePrice { get; private set; }

    public AssetStatus Status { get; private set; }

    public int LocationId { get; private set; }

    public Location? Location { get; private set; }

    public void UpdateDetails(
        string name,
        string manufacturer,
        string serialNumber,
        DateOnly purchasedOn,
        decimal purchasePrice,
        DateOnly today)
    {
        GuardNotRetired();
        ApplyDetails(name, manufacturer, serialNumber, purchasedOn, purchasePrice, today);
    }

    public void MoveTo(int locationId)
    {
        GuardNotRetired();
        LocationId = locationId;
    }

    public void ChangeStatus(AssetStatus status)
    {
        if (Status == status)
        {
            return;
        }

        GuardNotRetired();
        Status = status;
    }

    private void ApplyDetails(
        string name,
        string manufacturer,
        string serialNumber,
        DateOnly purchasedOn,
        decimal purchasePrice,
        DateOnly today)
    {
        Name = Ensure.MaxLength(Ensure.NotBlank(name, nameof(name)), 150, nameof(name));
        Manufacturer = Ensure.MaxLength(Ensure.NotBlank(manufacturer, nameof(manufacturer)), 100, nameof(manufacturer));
        SerialNumber = Ensure.MaxLength(Ensure.NotBlank(serialNumber, nameof(serialNumber)), 100, nameof(serialNumber));
        PurchasedOn = Ensure.NotInFuture(purchasedOn, today, nameof(purchasedOn));
        PurchasePrice = Ensure.NotNegative(purchasePrice, nameof(purchasePrice));
    }

    private void GuardNotRetired()
    {
        if (Status == AssetStatus.Retired)
        {
            throw new DomainValidationException("A retired asset can no longer be modified.");
        }
    }

    private static string NormaliseTag(string assetTag)
    {
        var trimmed = Ensure.MaxLength(Ensure.NotBlank(assetTag, nameof(assetTag)), 20, nameof(assetTag));
        return trimmed.ToUpperInvariant();
    }
}
