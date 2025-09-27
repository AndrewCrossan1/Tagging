using Live.AndrewCrossan.Tagging.Models;

namespace Live.AndrewCrossan.Tagging.Validation;

public class TagValidator<TTag> : ITagValidator<TTag>
    where TTag : class, ITag, new()
{
    public Dictionary<string, List<string>> Errors { get; set; }

    public TagValidator()
    {
        Errors = new Dictionary<string, List<string>>();
    }
    
    /// <summary>
    /// Asynchronously validates the tag.
    /// This method should be implemented to perform the actual validation logic.
    /// It should return true if the tag is valid, or false if it is not.
    /// If the tag is not valid, it should populate the Errors dictionary with appropriate error messages
    /// for each validation failure.
    /// </summary>
    /// <param name="tag">The model to validate</param>
    /// <param name="cancellationToken"></param>
    /// <returns>True, if the validation is successful, false otherwise</returns>
    public Task<bool> ValidateAsync(TTag tag, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrEmpty(tag.Name))
        {
            Errors[tag.Name] = new List<string> { "Tag name cannot be null." };
        }
        
        return Task.FromResult(Errors.Count == 0);
    }
}