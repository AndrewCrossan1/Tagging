using Live.AndrewCrossan.Tagging.Models;
using Microsoft.EntityFrameworkCore;

namespace Live.AndrewCrossan.Tagging.Context;

public class TaggingDbContext<TEntity, TJoin, TKey, TTag> : DbContext
    where TEntity : class, ITaggableModel<TKey>
    where TJoin : class, ITaggableEntity<TEntity, TKey, TTag>, new()
    where TTag : class, ITag
{
    public TaggingDbContext(DbContextOptions options) : base(options) {}
    
    public DbSet<TTag> Tags { get; set; }
    public DbSet<TJoin> TaggableEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // This is the crucial configuration for the many-to-many relationship.
        modelBuilder.Entity<TJoin>()
            .HasKey(tt => new { tt.EntityId, tt.TagId });
        
        modelBuilder.Entity<TJoin>()
            .HasOne(tt => tt.Tag)
            .WithMany()
            .HasForeignKey(tt => tt.TagId);
        
        modelBuilder.Entity<TJoin>()
            .HasOne(tt => tt.Entity)
            .WithMany()
            .HasForeignKey(tt => tt.EntityId);
    }

    public override int SaveChanges()
    {
        SetEntityProperties();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetEntityProperties();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetEntityProperties()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity<Guid> && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity<Guid>)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.Id = Guid.NewGuid();
                entity.CreatedAt = DateTime.UtcNow;
            }
            
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}