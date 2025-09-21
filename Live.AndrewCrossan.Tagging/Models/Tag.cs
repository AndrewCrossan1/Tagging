namespace Live.AndrewCrossan.Tagging.Models;

/// <summary>
/// <c>Tag</c> represents a tag that can be associated with various entities.
/// It includes properties for the tag's name, description, and slug.
/// Inherits from <c>BaseEntity</c> to include common entity properties.
/// </summary>
public class Tag : BaseEntity<Guid>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Slug { get; set; }
    
    public Tag()
    {
        Active = true;
    }
}
