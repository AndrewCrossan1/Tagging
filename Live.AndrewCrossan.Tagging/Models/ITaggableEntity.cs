namespace Live.AndrewCrossan.Tagging.Models;

/// <summary>
/// A generic join entity. This is used to link a <see cref="ITag"/> to any entity that implements <see cref="ITaggableModel"/>.
/// </summary>
/// <typeparam name="TEntity">The taggable entity</typeparam>
/// <typeparam name="TTag">The tag type</typeparam>
public interface ITaggableEntity<TEntity, TTag>
    where TTag : class, ITag
    where TEntity : class, ITaggableModel
{
    Guid EntityId { get; set;}
    TEntity Entity { get; set; }
    Guid TagId { get; set; }
    TTag Tag { get; set; }
}