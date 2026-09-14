using AssetRegistry.Domain.Enums;

namespace AssetRegistry.Application.Assets.Dtos;

public sealed record AssetDto(
    int Id,
    string AssetTag,
    string Name,
    string Manufacturer,
    string SerialNumber,
    DateOnly PurchasedOn,
    decimal PurchasePrice,
    AssetStatus Status,
    int LocationId,
    string LocationName);
