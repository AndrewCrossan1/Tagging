using System.ComponentModel.DataAnnotations;

namespace Live.AndrewCrossan.Tagging.Models;

public abstract class BaseTag : ITag
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Active { get; set; }

    protected BaseTag()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Active = true;
    }
}