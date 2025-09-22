namespace Live.AndrewCrossan.Tagging.Models;

/// <summary>
/// A contract which defines the minimum properties required for a custom tag model.
/// </summary>
public interface ITag
{
    Guid Id { get; set; }
    string Name { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}