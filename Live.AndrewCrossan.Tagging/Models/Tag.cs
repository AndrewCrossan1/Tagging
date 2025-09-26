using System.ComponentModel.DataAnnotations;

namespace Live.AndrewCrossan.Tagging.Models;

/// <summary>
/// <c>Tag</c> represents a tag that can be associated with various entities.
/// It includes properties for the tag's name, description, and slug.
/// Inherits from <c>BaseEntity</c> to include common entity properties.
/// </summary>
public class Tag : BaseTag
{
    [MaxLength(100)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Slug => Name?.ToLower().Replace(" ", "-") ?? string.Empty;
}
