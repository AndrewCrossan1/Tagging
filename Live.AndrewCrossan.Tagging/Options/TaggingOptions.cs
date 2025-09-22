namespace Live.AndrewCrossan.Tagging.Options;

public class TaggingOptions
{
    /// <summary>
    /// If true, allows tags to be created with the same name.
    /// If false, tag names must be unique.
    /// </summary>
    public bool AllowDuplicates { get; set; } = false;
    
    
}