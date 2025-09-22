using Live.AndrewCrossan.Tagging.Context;
using Live.AndrewCrossan.Tagging.Models;

namespace Live.AndrewCrossan.Tagging.Repositories;

public class TaggableRepository<TEntity, TJoin, TTag> : ITaggableRepository<TEntity, TJoin, TTag>
    where TEntity : class, ITaggableModel
    where TTag : class, ITag
    where TJoin : class, ITaggableEntity<TEntity, TTag>, new()
{
    private readonly TaggingDbContext<TEntity, TJoin, TTag> _context;
    
    public Task AddTagToEntityAsync(TEntity entity, TTag tag, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveTagFromEntityAsync(TEntity entity, TTag tag, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TTag>> GetTagsForEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetEntitiesForTagAsync(TTag tag, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EntityHasTagAsync(TEntity entity, TTag tag, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
}