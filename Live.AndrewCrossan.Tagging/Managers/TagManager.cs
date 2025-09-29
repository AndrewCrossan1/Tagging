using Live.AndrewCrossan.Tagging.Models;
using Live.AndrewCrossan.Tagging.Options;
using Live.AndrewCrossan.Tagging.Validation;
using Live.AndrewCrossan.Tagging.Repositories;
using Live.AndrewCrossan.Tagging.Logging;
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
    private readonly ILogger _logger;
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
    /// <param name="tags">A collection of tag titles</param>
    /// <exception cref="TagValidationException">Thrown when validation fails, with 'Errors' property providing a description for each</exception>
    /// <returns>A list of existing or newly created tags</returns>
    public async Task<ICollection<TTag>> CreateOrGetTagsAsync(IEnumerable<TTag> tags)
    {
        var uniqueTagNames = tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag.Name))
            .Select(tag => tag.Name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        if (uniqueTagNames.Count > _options.MaximumTagsPerEntity) 
        {
            _logger.TagLimitExceeded(_options.MaximumTagsPerEntity, uniqueTagNames.Count);
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
            _logger.TagValidationFailed(newTag.Name, _validator.Errors);
            throw new TagValidationException("Tag validation failed", _validator.Errors);
        }

        if (newTags.Count > 0)
        {
            _logger.CreatingNewTags(newTags.Count);
            await _repository.SaveRangeAsync(newTags);
        }

        _logger.ReturningTotalTags(existingTags.Count + newTags.Count);
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
            _logger.TagValidationFailed(tag.Name, _validator.Errors);
            throw new TagValidationException("Tag validation failed", _validator.Errors);
        }

        var existingTag = await _repository.TagExistsAsync(tag.Name);
        if (existingTag != null)
        {
            _logger.TagAlreadyExists(tag.Name);
            validationResult = await _validator.ValidateAsync(tag);
            if (validationResult) return await _repository.SaveTagAsync(existingTag, tag);
            _logger.TagValidationFailed(tag.Name, _validator.Errors);
            throw new TagValidationException("Tag validation failed", _validator.Errors);
        }
        
        _logger.CreatingNewTag(tag.Name);

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
            _logger.DeletingNonExistentTag(tagId);
            throw new KeyNotFoundException($"Tag with ID {tagId} not found.");
        }
        
        _logger.DeletingTag(tagId);

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
            _logger.TagRetrieved(tagId);
        }
        else
        {
            _logger.TagNotFound(tagId);
        }
        
        return tag ?? throw new KeyNotFoundException($"Tag with ID {tagId} not found.");
    }
}