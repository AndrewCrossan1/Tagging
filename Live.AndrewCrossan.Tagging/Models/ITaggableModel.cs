namespace Live.AndrewCrossan.Tagging.Models;

/// <summary>
/// A contract that defines the minimum requirements for a model to be taggable.
/// </summary>
public interface ITaggableModel
{
    Guid Id { get; set; }
}