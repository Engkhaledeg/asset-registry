using System.ComponentModel.DataAnnotations;
using AssetRegistry.Domain.Enums;

namespace AssetRegistry.Application.Assets.Dtos;

public sealed class UpdateAssetRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Manufacturer { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string SerialNumber { get; init; } = string.Empty;

    [Required]
    public DateOnly PurchasedOn { get; init; }

    [Range(0, 1_000_000)]
    public decimal PurchasePrice { get; init; }

    [Range(1, int.MaxValue)]
    public int LocationId { get; init; }

    [EnumDataType(typeof(AssetStatus))]
    public AssetStatus Status { get; init; }
}
