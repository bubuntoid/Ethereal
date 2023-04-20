using Ethereal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Domain;

public class EtherealDbContext : DbContext
{
    private readonly IDatabaseSettings databaseSettings;

    public EtherealDbContext(IDatabaseSettings databaseSettings)
    {
        this.databaseSettings = databaseSettings;
    }

    public DbSet<ProcessingJob> ProcessingJobs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(databaseSettings.ConnectionString);
        optionsBuilder.UseCamelCaseNamingConvention();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EtherealDbContext).Assembly);
        modelBuilder.HasDefaultSchema("public");
        base.OnModelCreating(modelBuilder);
    }
}