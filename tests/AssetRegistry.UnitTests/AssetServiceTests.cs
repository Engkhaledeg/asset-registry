using AssetRegistry.Application.Assets;
using AssetRegistry.Application.Assets.Dtos;
using AssetRegistry.Application.Exceptions;
using AssetRegistry.UnitTests.Fakes;
using Xunit;

namespace AssetRegistry.UnitTests;

public class AssetServiceTests
{
    private static readonly DateOnly Today = new(2026, 1, 15);

    private readonly FakeAssetRepository _assets = new();
    private readonly FakeLocationRepository _locations = new(1, 2);
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task CreateAsync_StoresTheAsset()
    {
        var service = CreateService();

        var created = await service.CreateAsync(BuildRequest(), CancellationToken.None);

        Assert.Equal("LAP-001", created.AssetTag);
        Assert.Single(_assets.Assets);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task CreateAsync_RejectsADuplicateAssetTag()
    {
        var service = CreateService();
        await service.CreateAsync(BuildRequest(), CancellationToken.None);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateAsync(BuildRequest(assetTag: "lap-001"), CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_RejectsAnUnknownLocation()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.CreateAsync(BuildRequest(locationId: 99), CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_ReportsMissingAssets()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(404, CancellationToken.None));
    }

    private AssetService CreateService()
    {
        return new AssetService(_assets, _locations, _unitOfWork, new FixedClock(Today));
    }

    private static CreateAssetRequest BuildRequest(string assetTag = "LAP-001", int locationId = 1)
    {
        return new CreateAssetRequest
        {
            AssetTag = assetTag,
            Name = "ThinkPad T14",
            Manufacturer = "Lenovo",
            SerialNumber = "SN-12345",
            PurchasedOn = Today.AddYears(-1),
            PurchasePrice = 1299.00m,
            LocationId = locationId
        };
    }
}
