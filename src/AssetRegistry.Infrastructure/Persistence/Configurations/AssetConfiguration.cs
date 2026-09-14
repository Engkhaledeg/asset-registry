using AssetRegistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetRegistry.Infrastructure.Persistence.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(asset => asset.Id);

        builder.Property(asset => asset.AssetTag)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(asset => asset.AssetTag)
            .IsUnique();

        builder.Property(asset => asset.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(asset => asset.Manufacturer)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(asset => asset.SerialNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(asset => asset.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(asset => asset.Status)
            .HasConversion<int>();

        builder.HasOne(asset => asset.Location)
            .WithMany(location => location.Assets)
            .HasForeignKey(asset => asset.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(asset => asset.Status);
    }
}
