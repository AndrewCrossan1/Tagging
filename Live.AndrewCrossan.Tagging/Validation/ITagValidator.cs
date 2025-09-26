using Live.AndrewCrossan.Tagging.Models;

namespace Live.AndrewCrossan.Tagging.Validation;

public interface ITagValidator<TTag> where TTag : ITag, new()
{
    Dictionary<string, List<string>> Errors { get; set; }
    
    Task<bool> ValidateAsync(TTag tag, CancellationToken cancellationToken = default);
}