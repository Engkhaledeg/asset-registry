using System.ComponentModel.DataAnnotations;

namespace AssetRegistry.Application.Assets.Dtos;

public sealed class CreateAssetRequest
{
    [Required]
    [MaxLength(20)]
    public string AssetTag { get; init; } = string.Empty;

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
}
