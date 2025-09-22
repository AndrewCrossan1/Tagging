namespace Live.AndrewCrossan.Tagging.Models;

/// <summary>
/// A base entity that provides common properties for all entities.
/// This includes an Id, CreatedAt, UpdatedAt, and Active status.
/// This ensures consistency across all entities in the system.
/// </summary>
/// <typeparam name="TJoin">The Tag model to use</typeparam>
public class BaseEntity<TJoin>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Active { get; set; }
    
    public ICollection<TJoin> Tags { get; set; }
    
    protected BaseEntity()
    {
        Active = true;
    }
}