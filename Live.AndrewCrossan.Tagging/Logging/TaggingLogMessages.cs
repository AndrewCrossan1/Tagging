using Microsoft.Extensions.Logging;

namespace Live.AndrewCrossan.Tagging.Logging;

public static partial class TaggingLogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Tagging started")]
    public static partial void TaggingStarted(this ILogger logger);
    
    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Tagging completed")]
    public static partial void TaggingCompleted(this ILogger logger);
    
    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Tagging failed: {ErrorMessage}")]
    public static partial void TaggingFailed(this ILogger logger, string errorMessage);
    
    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Tag limit exceeded. Maximum allowed is {MaxTags}, but {ProvidedTags} were provided.")]
    public static partial void TagLimitExceeded(this ILogger logger, int maxTags, int providedTags);
    
    [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "Tag validation failed for tag '{TagName}', Errors: {Errors}")]
    public static partial void TagValidationFailed(this ILogger logger, string tagName, Dictionary<string, List<string>> errors);
    
    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Creating {Count} new tags.")]
    public static partial void CreatingNewTags(this ILogger logger, int count);
    
    [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Returning a total of {TotalCount} tags (existing and new).")]
    public static partial void ReturningTotalTags(this ILogger logger, int totalCount);
    
    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "Tag with name {TagName} already exists. Returning existing tag.")]
    public static partial void TagAlreadyExists(this ILogger logger, string tagName);
    
    [LoggerMessage(EventId = 8, Level = LogLevel.Information, Message = "Creating new tag with name: {TagName}")]
    public static partial void CreatingNewTag(this ILogger logger, string tagName);
    
    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "Attempted to delete non-existent tag with ID: {TagId}")]
    public static partial void DeletingNonExistentTag(this ILogger logger, Guid tagId);
    
    [LoggerMessage(EventId = 10, Level = LogLevel.Information, Message = "Deleting tag with ID: {TagId}")]
    public static partial void DeletingTag(this ILogger logger, Guid tagId);
    
    [LoggerMessage(EventId = 11, Level = LogLevel.Information, Message = "Tag with ID {TagId} retrieved successfully.")]
    public static partial void TagRetrieved(this ILogger logger, Guid tagId);
    
    [LoggerMessage(EventId = 12, Level = LogLevel.Warning, Message = "Tag with ID {TagId} not found.")]
    public static partial void TagNotFound(this ILogger logger, Guid tagId);
    
    
}