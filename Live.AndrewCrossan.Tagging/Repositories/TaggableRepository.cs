using Live.AndrewCrossan.Tagging.Context;
using Live.AndrewCrossan.Tagging.Models;
using Microsoft.EntityFrameworkCore;

namespace Live.AndrewCrossan.Tagging.Repositories;

public class TaggableRepository<TEntity, TJoin, TTag> : ITaggableRepository<TEntity, TJoin, TTag>
    where TEntity : class, ITaggableModel
    where TTag : class, ITag
    where TJoin : class, ITaggableEntity<TEntity, TTag>, new()
{
    private readonly TaggingDbContext<TEntity, TJoin, TTag> _context;
    
    public TaggableRepository(TaggingDbContext<TEntity, TJoin, TTag> context)
    {
        _context = context;
    }

    /// <summary>
    /// Add tag(s) to an entity
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="tag">The list of tags</param>
    /// <param name="cancellationToken"></param>
    public async Task AddTagToEntityAsync(TEntity entity, ICollection<TTag> tag, CancellationToken cancellationToken = default)
    {
        foreach (var t in tag)
        {
            var join = new TJoin
            {
                Entity = entity,
                Tag = t
            };
            await _context.Set<TJoin>().AddAsync(join, cancellationToken);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Remove a tag from an entity
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="tag">The tag</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A number representing the rows affected by the operation</returns>
    /// <exception cref="InvalidOperationException">If a tag is not found on the entity, this will be thrown.</exception>
    public async Task<int> RemoveTagFromEntityAsync(TEntity entity, TTag tag, CancellationToken cancellationToken = default)
    {
        var join = _context.Set<TJoin>().FirstOrDefault(j => j.EntityId == entity.Id && j.TagId == tag.Id);
        if (join == null) throw new InvalidOperationException("Tag not found on entity");
        
        _context.Set<TJoin>().Remove(join);
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TTag>> GetTagsForEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var tags = await _context.Set<TJoin>()
            .Where(j => j.EntityId == entity.Id)
            .Select(j => j.Tag)
            .ToListAsync(cancellationToken);    
        return tags;
    }

    public async Task<List<TEntity>> GetEntitiesForTagAsync(TTag tag, CancellationToken cancellationToken = default)
    {
        // Get all entities for a given tag
        var entities = await _context.Set<TJoin>()
            .Where(j => j.TagId == tag.Id)
            .Select(j => j.Entity)
            .ToListAsync(cancellationToken);
        return entities;
    }

    public async Task<bool> EntityHasTagAsync(TEntity entity, TTag tag, CancellationToken cancellation = default)
    {
        // Check if an entity has a given tag
        var hasTag = await _context.Set<TJoin>()
            .AnyAsync(j => j.EntityId == entity.Id && j.TagId == tag.Id, cancellation);
        return hasTag;
    }
}