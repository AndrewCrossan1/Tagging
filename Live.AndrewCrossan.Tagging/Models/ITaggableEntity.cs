namespace Live.AndrewCrossan.Tagging.Models;

/// <summary>
/// A generic join entity. This is used to link a <see cref="ITag"/> to any entity that implements <see cref="ITaggableModel{TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">The taggable entity</typeparam>
/// <typeparam name="TKey">The type of primary key</typeparam>
/// <typeparam name="TTag">The tag type</typeparam>
public interface ITaggableEntity<TEntity, TKey, TTag>
    where TTag : class, ITag
    where TEntity : class, ITaggableModel<TKey>
{
    TKey EntityId { get; set;}
    TEntity Entity { get; set; }
    Guid TagId { get; set; }
    TTag Tag { get; set; }
}