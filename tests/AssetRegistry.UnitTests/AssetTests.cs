using AssetRegistry.Domain.Entities;
using AssetRegistry.Domain.Enums;
using AssetRegistry.Domain.Exceptions;
using Xunit;

namespace AssetRegistry.UnitTests;

public class AssetTests
{
    private static readonly DateOnly Today = new(2026, 1, 15);

    [Fact]
    public void Constructor_UppercasesTheAssetTag()
    {
        var asset = CreateAsset(assetTag: "lap-014");

        Assert.Equal("LAP-014", asset.AssetTag);
    }

    [Fact]
    public void Constructor_StartsTheAssetInStock()
    {
        var asset = CreateAsset();

        Assert.Equal(AssetStatus.InStock, asset.Status);
    }

    [Fact]
    public void Constructor_RejectsAPurchaseDateInTheFuture()
    {
        var futureDate = Today.AddDays(1);

        Assert.Throws<DomainValidationException>(() => CreateAsset(purchasedOn: futureDate));
    }

    [Fact]
    public void ChangeStatus_RejectsChangesOnceRetired()
    {
        var asset = CreateAsset();
        asset.ChangeStatus(AssetStatus.Retired);

        Assert.Throws<DomainValidationException>(() => asset.ChangeStatus(AssetStatus.Deployed));
    }

    [Fact]
    public void MoveTo_RejectsRelocationOnceRetired()
    {
        var asset = CreateAsset();
        asset.ChangeStatus(AssetStatus.Retired);

        Assert.Throws<DomainValidationException>(() => asset.MoveTo(2));
    }

    private static Asset CreateAsset(string assetTag = "LAP-001", DateOnly? purchasedOn = null)
    {
        return new Asset(
            assetTag,
            "ThinkPad T14",
            "Lenovo",
            "SN-12345",
            purchasedOn ?? Today.AddYears(-1),
            1299.00m,
            locationId: 1,
            today: Today);
    }
}
