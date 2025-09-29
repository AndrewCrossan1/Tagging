namespace Live.AndrewCrossan.Tagging.Validation;

public class TagValidationException : Exception
{
    public Dictionary<string, List<string>> Errors { get; set; }
    
    public TagValidationException(string message, Dictionary<string, List<string>> errors) : base(message)
    {
        Errors = errors;
    }
}