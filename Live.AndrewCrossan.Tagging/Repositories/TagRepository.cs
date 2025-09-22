using Live.AndrewCrossan.Tagging.Context;
using Live.AndrewCrossan.Tagging.Models;

namespace Live.AndrewCrossan.Tagging.Repositories;

public class TagRepository<TEntity, TJoin, TTag> : ITagRepository<TEntity, TJoin, TTag>
    where TTag : class, ITag
    where TEntity : class, ITaggableModel
    where TJoin : class, ITaggableEntity<TEntity, TTag>, new()
{

    private readonly TaggingDbContext<TEntity, TJoin, TTag> _context;
    
    public Task<List<TTag>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TTag?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TTag?> GetTagByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TTag> CreateTagAsync(TTag tag, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TTag> UpdateTagAsync(TTag tag, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTagAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TagExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TagNameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveRangeAsync(IEnumerable<TTag> tags, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}