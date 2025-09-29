using Live.AndrewCrossan.Tagging.Models;

namespace Live.AndrewCrossan.Tagging.Repositories;

public interface ITagRepository<TTag> 
    where TTag : class, ITag
{
    Task<ICollection<TTag>> GetAllTagsAsync(CancellationToken cancellationToken = default);
    Task<TTag?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TTag> CreateTagAsync(TTag tag, CancellationToken cancellationToken = default);
    Task<TTag> DeleteTagAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TTag?> TagExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TTag?> TagExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<List<TTag>> GetTagsByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);
    Task SaveRangeAsync(IEnumerable<TTag> tags, CancellationToken cancellationToken = default);
    Task<TTag> SaveTagAsync(TTag tag, TTag newTag, CancellationToken cancellationToken = default);   
}