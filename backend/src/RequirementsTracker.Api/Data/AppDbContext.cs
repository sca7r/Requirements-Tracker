using Microsoft.EntityFrameworkCore;
using RequirementsTracker.Api.Models;

namespace RequirementsTracker.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Requirement> Requirements => Set<Requirement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(e =>
        {
            e.HasIndex(p => p.Name);
            e.HasMany(p => p.Requirements)
                .WithOne(r => r.Project)
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Requirement>(e =>
        {
            e.Property(r => r.EstimatedHours).HasPrecision(10, 2);
            e.Property(r => r.Type).HasConversion<string>().HasMaxLength(32);
            e.Property(r => r.Priority).HasConversion<string>().HasMaxLength(32);
            e.Property(r => r.Status).HasConversion<string>().HasMaxLength(32);
            e.HasIndex(r => new { r.ProjectId, r.Status });
        });
    }
}
