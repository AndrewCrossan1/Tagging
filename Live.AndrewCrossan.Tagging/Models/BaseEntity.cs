namespace Live.AndrewCrossan.Tagging.Models;

/// <summary>
/// A base entity that provides common properties for all entities.
/// This includes an Id, CreatedAt, UpdatedAt, and Active status.
/// This ensures consistency across all entities in the system.
/// </summary>
/// <typeparam name="TKey">The Primary Key Type</typeparam>
public class BaseEntity<TKey, TJoin>
{
    public TKey Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Active { get; set; }
    
    public ICollection<TJoin> Tags { get; set; }
    
    protected BaseEntity()
    {
        Active = true;
    }
}