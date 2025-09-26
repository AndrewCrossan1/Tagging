using Live.AndrewCrossan.Tagging.Models;
using Live.AndrewCrossan.Tagging.Options;
using Live.AndrewCrossan.Tagging.Validation;
using Live.AndrewCrossan.Tagging.Repositories;
using Microsoft.Extensions.Logging;

namespace Live.AndrewCrossan.Tagging.Managers;

/// <summary>
/// Tag Manager acts as a service layer between the controllers and the repositories.
/// It contains business logic and implements validation rules provided by you, or the default
/// ones provided by the library. For custom validations see <see cref="ITagValidator{TTag}"/>.
/// </summary>
/// <typeparam name="TTag">The tag entity type.</typeparam>
public class TagManager<TTag>  
    where TTag : class, ITag, new()
{
    private readonly ITagRepository<TTag> _repository;
    private readonly ITagValidator<TTag> _validator;
    private readonly ILogger<TagManager<TTag>> _logger;
    private readonly TaggingOptions _options;
    
    public TagManager(ILogger<TagManager<TTag>> logger, ITagRepository<TTag> repository, ITagValidator<TTag> validator,TaggingOptions options)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _options = options;
    }

    /// <summary>
    /// Creates or retrieves tags based on the provided titles.
    /// If a tag with the given title already exists, it is retrieved; otherwise, a new tag is created.
    /// The method ensures that all tags conform to the validation rules defined in the <see cref="ITagValidator{TTag}"/>.
    /// If any tag title is invalid, an exception is thrown and no tags are created or retrieved.
    /// This method is useful for batch operations where multiple tags need to be processed at once.
    /// </summary>
    /// <param name="tagNames">A collection of tag titles</param>
    /// <exception cref="TagValidationException">Thrown when validation fails, with 'Errors' property providing a description for each</exception>
    /// <returns>A list of existing or newly created tags</returns>
    public async Task<ICollection<TTag>> CreateOrGetTagsAsync(IEnumerable<string> tagNames)
    {
        var uniqueTagNames = tagNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        if (uniqueTagNames.Count > _options.MaximumTagsPerEntity) 
        {
            _logger.LogWarning("Tag limit exceeded. Maximum allowed is {MaxTags}, but {ProvidedTags} were provided.", _options.MaximumTagsPerEntity, uniqueTagNames.Count);
            throw new TagConfigurationException($"A maximum of {_options.MaximumTagsPerEntity} tags are allowed per entity.");
        }

        var existingTags = await _repository.GetTagsByNamesAsync(uniqueTagNames);
        var existingTagNames = existingTags.Select(t => t.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        var newTags = uniqueTagNames // Use the clean input list
            .Where(title => !existingTagNames.Contains(title)) // Only keep names NOT found in the DB
            .Select(title => new TTag { Name = title })
            .ToList();

        foreach (var newTag in newTags)
        {
            if (await _validator.ValidateAsync(newTag)) continue;
            _logger.LogWarning("Tag validation failed for new tag: {TagName}", newTag.Name);
            throw new TagValidationException("Tag validation failed", _validator.Errors);
        }

        if (newTags.Count > 0)
        {
            _logger.LogInformation("Creating {Count} new tags.", newTags.Count);
            await _repository.SaveRangeAsync(newTags);
        }

        _logger.LogInformation("Returning a total of {TotalCount} tags (existing and new).", existingTags.Count + newTags.Count);
        return existingTags.Union(newTags).ToList();
    }
    
    /// <summary>
    /// Saves a tag after validating it.
    /// If a tag with the same title already exists, it is returned instead of creating a new one.
    /// Validation is performed using the <see cref="ITagValidator{TTag}"/>.
    /// If validation fails, a <see cref="TagValidationException"/> is thrown with details of the validation errors.
    /// This method ensures that duplicate tags are not created and that all tags conform to the defined validation rules.
    /// </summary>
    /// <param name="tag">The tag to be saved</param>
    /// <returns>The saved tag</returns>
    /// <exception cref="TagValidationException">Thrown if a tag fails validation</exception>
    public async Task<TTag> SaveTagAsync(TTag tag)
    {
        var validationResult = await _validator.ValidateAsync(tag);
        if (!validationResult)
        {
            _logger.LogWarning("Tag validation failed for tag: {TagName}. Errors: {Errors}", tag.Name, _validator.Errors);
            throw new TagValidationException("Tag validation failed", _validator.Errors);
        }

        var existingTag = await _repository.TagExistsAsync(tag.Name);
        if (existingTag != null)
        {
            _logger.LogInformation("Tag with name {TagName} already exists. Returning existing tag.", tag.Name);
            return existingTag;
        }
        
        _logger.LogInformation("Creating new tag with name: {TagName}", tag.Name);

        return await _repository.CreateTagAsync(tag);
    }
    
    /// <summary>
    ///  Deletes a tag by its ID.
    ///  If the tag does not exist, a <see cref="KeyNotFoundException"/> is thrown.
    ///  This method ensures that only existing tags can be deleted, preventing errors
    ///  related to non-existent resources.
    /// </summary>
    /// <param name="tagId">The tag id to delete</param>
    /// <exception cref="KeyNotFoundException">Thrown if a tag is not found with the given ID</exception>
    /// <returns>The deleted tag</returns>
    public async Task<TTag> DeleteTagAsync(Guid tagId)
    {
        var existingTag = await _repository.TagExistsAsync(tagId);
        if (existingTag == null)
        {
            _logger.LogWarning("Attempted to delete non-existent tag with ID: {TagId}", tagId);
            throw new KeyNotFoundException($"Tag with ID {tagId} not found.");
        }
        
        _logger.LogInformation("Deleting tag with ID: {TagId}", tagId);

        return await _repository.DeleteTagAsync(tagId);
    }
    
    /// <summary>
    /// Gets all tags.
    /// This method retrieves all tags from the repository.
    /// </summary>
    /// <returns>A collection of tags</returns>
    public async Task<ICollection<TTag>> GetAllTagsAsync()
    {
        _logger.LogInformation("Retrieving all tags.");
        return await _repository.GetAllTagsAsync();
    }
    
    /// <summary>
    /// Gets a tag by its ID.
    /// If the tag does not exist, null is returned.
    /// </summary>
    /// <param name="tagId">The tag id to retrieve</param>
    /// <exception cref="KeyNotFoundException">Thrown if a tag is not found with the given ID</exception>
    /// <returns>The tag if found, otherwise null</returns>
    public async Task<TTag?> GetTagByIdAsync(Guid tagId)
    {
        var tag = await _repository.GetTagByIdAsync(tagId);
        
        if (tag != null)
        {
            _logger.LogInformation("Tag with ID {TagId} retrieved successfully.", tagId);
        }
        else
        {
            _logger.LogWarning("Tag with ID {TagId} not found.", tagId);
        }
        
        return tag ?? throw new KeyNotFoundException($"Tag with ID {tagId} not found.");
    }
}