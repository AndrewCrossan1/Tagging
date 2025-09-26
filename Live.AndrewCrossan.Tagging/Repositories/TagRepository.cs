using Live.AndrewCrossan.Tagging.Models;
using Microsoft.EntityFrameworkCore;

namespace Live.AndrewCrossan.Tagging.Repositories;

public class TagRepository<TTag> : ITagRepository<TTag>
    where TTag : class, ITag
{

    private readonly DbContext _context;
    
    public TagRepository(DbContext context)
    {
        _context = context;
    }
    
    public Task<ICollection<TTag>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TTag?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken = default)
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

    public Task<TTag> DeleteTagAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TTag?> TagExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public Task<TTag?> TagExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<TTag>> GetTagsByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveRangeAsync(IEnumerable<TTag> tags, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}