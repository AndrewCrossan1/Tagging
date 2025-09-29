using Live.AndrewCrossan.Tagging.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Live.AndrewCrossan.Tagging.Repositories;

public class TagRepository<TTag> : ITagRepository<TTag>
    where TTag : class, ITag
{

    private readonly DbContext _context;
    
    public TagRepository(DbContext context)
    {
        _context = context;
    }
    
    public async Task<ICollection<TTag>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TTag>().ToListAsync(cancellationToken);
    }

    public async Task<TTag?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TTag>().FindAsync(id, cancellationToken);
    }

    public async Task<TTag> CreateTagAsync(TTag tag, CancellationToken cancellationToken = default)
    {
        var r = await _context.Set<TTag>().AddAsync(tag, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
        return r.Entity;
    }

    public async Task<TTag> UpdateTagAsync(TTag tag, CancellationToken cancellationToken = default)
    {
        var r = _context.Set<TTag>().Update(tag);
        
        await _context.SaveChangesAsync(cancellationToken);
        return r.Entity;
    }

    public async Task<TTag> DeleteTagAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _context.Set<TTag>().FindAsync(id, cancellationToken);
        
        _context.Set<TTag>().Remove(tag!);

        await _context.SaveChangesAsync(cancellationToken);
        return tag!;
    }

    public async Task<TTag?> TagExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TTag>().FindAsync(id, cancellationToken);
    }
    
    public async Task<TTag?> TagExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TTag>().AsQueryable().SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<List<TTag>> GetTagsByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TTag>().AsQueryable().Where(x => names.Contains(x.Name)).ToListAsync(cancellationToken);
    }

    public async Task SaveRangeAsync(IEnumerable<TTag> tags, CancellationToken cancellationToken = default)
    {
        await _context.Set<TTag>().AddRangeAsync(tags, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TTag> SaveTagAsync(TTag tag, TTag newTag, CancellationToken cancellationToken = default)
    {
        _context.Set<TTag>().Update(tag);

        tag.Name = newTag.Name;
        
        await _context.SaveChangesAsync(cancellationToken);
        return newTag;
    }
}