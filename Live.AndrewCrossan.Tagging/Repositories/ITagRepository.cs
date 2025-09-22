using Live.AndrewCrossan.Tagging.Context;
using Live.AndrewCrossan.Tagging.Models;

namespace Live.AndrewCrossan.Tagging.Repositories;

public interface ITagRepository<TEntity, TJoin, TTag> 
    where TTag : class, ITag
    where TEntity : class, ITaggableModel
    where TJoin : class, ITaggableEntity<TEntity, TTag>, new()
{
    Task<List<TTag>> GetAllTagsAsync(CancellationToken cancellationToken = default);
    Task<TTag?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TTag> CreateTagAsync(TTag tag, CancellationToken cancellationToken = default);
    Task<TTag> UpdateTagAsync(TTag tag, CancellationToken cancellationToken = default);
    Task DeleteTagAsync(Guid id, CancellationToken cancellationToken = default);
}