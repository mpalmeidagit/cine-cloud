using CineCloud.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CineCloud.Infrastructure.Context;

public class CineCloudWriteContext : DbContext
{
    public CineCloudWriteContext()
    {

    }
    public CineCloudWriteContext(DbContextOptions<CineCloudWriteContext> options) : base(options)
    {

    }

    public DbSet<Dvd> Dvds { get; set; }
    public DbSet<Director> Directors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
            e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
            property.SetColumnType("varchar(100)");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CineCloudWriteContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}