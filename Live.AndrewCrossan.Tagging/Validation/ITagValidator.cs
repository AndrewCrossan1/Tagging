using Live.AndrewCrossan.Tagging.Models;

namespace Live.AndrewCrossan.Tagging.Validation;

/// <summary>
/// Interface for validating tags in the system, can be used to enforce rules or constraints on tags.
/// This is useful for ensuring that tags meet certain criteria before they are added to entities.
/// Implementations can provide custom validation logic, such as checking for duplicates, length, or format
/// </summary>
/// <typeparam name="TTag">The tag model used</typeparam>
public interface ITagValidator<TTag> where TTag : class, ITag, new()
{
    Dictionary<string, List<string>> Errors { get; set; }
    
    Task<bool> ValidateAsync(TTag tag, CancellationToken cancellationToken = default);
}