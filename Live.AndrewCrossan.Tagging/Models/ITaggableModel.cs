namespace Live.AndrewCrossan.Tagging.Models;

/// <summary>
/// A contract that defines the minimum requirements for a model to be taggable.
/// </summary>
/// <typeparam name="TKey">The type of primary key</typeparam>
public interface ITaggableModel<TKey>
{
    TKey Id { get; set; }
}