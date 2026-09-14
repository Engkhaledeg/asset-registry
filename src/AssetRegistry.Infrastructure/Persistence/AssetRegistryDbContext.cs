using AssetRegistry.Application.Abstractions;
using AssetRegistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetRegistry.Infrastructure.Persistence;

public class AssetRegistryDbContext : DbContext, IUnitOfWork
{
    public AssetRegistryDbContext(DbContextOptions<AssetRegistryDbContext> options) : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<Location> Locations => Set<Location>();

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssetRegistryDbContext).Assembly);
    }
}
