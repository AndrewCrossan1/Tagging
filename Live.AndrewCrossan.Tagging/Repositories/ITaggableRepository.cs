using Live.AndrewCrossan.Tagging.Models;

namespace Live.AndrewCrossan.Tagging.Repositories;

public interface ITaggableRepository<TEntity, TJoin, TTag>
    where TEntity : class, ITaggableModel
    where TJoin : class, ITaggableEntity<TEntity, TTag>, new()
    where TTag : class, ITag
{
    Task AddTagToEntityAsync(TEntity entity, TTag tag, CancellationToken cancellationToken = default);
    Task RemoveTagFromEntityAsync(TEntity entity, TTag tag, CancellationToken cancellationToken = default);
    Task<List<TTag>> GetTagsForEntityAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetEntitiesForTagAsync(TTag tag, CancellationToken cancellationToken = default);
    Task<bool> EntityHasTagAsync(TEntity entity, TTag tag, CancellationToken cancellation = default);
}