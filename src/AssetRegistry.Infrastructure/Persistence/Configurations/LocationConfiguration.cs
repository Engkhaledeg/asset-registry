using AssetRegistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetRegistry.Infrastructure.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(location => location.Id);

        builder.Property(location => location.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(location => location.Building)
            .HasMaxLength(100)
            .IsRequired();

        builder.Metadata
            .FindNavigation(nameof(Location.Assets))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
