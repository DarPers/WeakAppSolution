using DataProcessorService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataProcessorService.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<SensorEvent> SensorEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SensorEvent>(entity =>
        {
            entity.Property(e => e.Payload).HasColumnType("jsonb");
        });
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var modifiedEntities = ChangeTracker.Entries()
            .Where(i => i.State == EntityState.Modified || i.State == EntityState.Added);

        foreach (var modifiedEntity in modifiedEntities)
        {
            var entity = (BaseEntity)modifiedEntity.Entity;

            switch (modifiedEntity.State)
            {
                case EntityState.Added:
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entity.UpdatedAt = DateTime.UtcNow;
                    modifiedEntity.Property("CreatedAt").IsModified = false;
                    break;
            }
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
