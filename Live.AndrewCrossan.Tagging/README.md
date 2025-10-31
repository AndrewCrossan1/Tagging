# Live.AndrewCrossan.Tagging
A simple tagging library for .NET applications using generics, allowing you to add and manage tags for any type of object.

![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/andrewcrossan1/Tagging/.github/workflows/dotnet.yml?branch=master)
![GitHub Release Date](https://img.shields.io/github/release-date/AndrewCrossan1/Tagging)
![GitHub Tag](https://img.shields.io/github/v/tag/AndrewCrossan1/Tagging?label=latest%20release)
![NuGet](https://img.shields.io/nuget/v/Live.AndrewCrossan.Tagging)

## Overview
The Live.AndrewCrossan.Tagging library provides a flexible and easy-to-use tagging system for
ASP.NET applications. It allows developers to add, remove, and retrieve tags for any type of object using a generic approach. This library is designed to be lightweight and efficient, making it suitable for a wide range of applications.

Currently, the library supports tagging for one group at a time, with plans to expand this functionality in future releases.

This is purely a passion project of mine that I thought of while trying to make a todo-list application. I wanted to be able to tag my tasks with various labels, but I couldn't find a simple and generic tagging library that would work for my needs. So, I decided to create one myself and share it with the community.

This package is not perfect, but I hope it can be useful to others who are looking for a simple tagging solution. If you have any feedback or suggestions, please feel free to reach out to me.

I intended to use generics as much as possible to allow the framework to be used for how you want it to be used, rather than how I think it should be used.

## Features
- Generic tagging system that works with any object type.
- Add, remove, and retrieve tags for objects.
- Easy integration into existing .NET applications.
- Lightweight and efficient.
- Open source and actively maintained.

## Contents
- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)
- [Additional Usage Examples](#additional-usage-examples)
- [Logging](#logging)
- [Contact](#contact)

## Requirements
- .NET 9.0 or later
- Entity Framework Core 9.0 or later
- Probably ASP.NET Core 9.0 or later (for web applications)
- A database provider supported by Entity Framework Core (e.g., SQL Server, SQLite, PostgreSQL, etc.)
- A basic understanding of generics in C# and Entity Framework Core.
- Familiarity with dependency injection in .NET applications.
- Basic knowledge of how to set up and configure a DbContext in Entity Framework Core.

## Installation
You can install the Live.AndrewCrossan.Tagging package via NuGet Package Manager Console:
```
Install-Package Live.AndrewCrossan.Tagging
```
Or via .NET CLI:
```
dotnet add package Live.AndrewCrossan.Tagging
```

Alternatively, you can use the NuGet Package Manager in Visual Studio to search for and install the package.

## Usage
As of version 1.0.0, tagging can only be added to one group at a time. Here's a simple example of how to use the tagging library:

### 1. Define Your Models

To use the extension, you first need three custom classes that implement the required interfaces (assuming your main application entity is Product):
```csharp
// 1. The main entity you want to tag
public class Product : ITaggableModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // ITaggableModel requires the Tags collection to be defined.
    public ICollection<ProductTagJoin> Tags { get; set; } = new HashSet<ProductTagJoin>();
}

// 2. The join table model (TJoin)
public class ProductTagJoin : ITaggableEntity<Product, Tag>
{
    // Required foreign keys
    public Guid EntityId { get; set; }
    public Guid TagId { get; set; }

    // Required navigation properties
    public Product Entity { get; set; }
    public Tag Tag { get; set; }
}

// 3. The tag model (TTag) (NOTE! This is optional, as you can use the built-in Tag class instead)
public class Tag : ITag
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // ITag requires the joined entities collection.
    public ICollection<ProductTagJoin> JoinedEntities { get; set; } = new HashSet<ProductTagJoin>();
}
```

### 2. Configure Services in Program.cs
```csharp
using Microsoft.EntityFrameworkCore;
using YourTaggingExtensionName.Services; // Ensure you have this namespace

var builder = WebApplication.CreateBuilder(args);

// Register the Tagging System
builder.Services.AddTagging<Product, ProductTagJoin, Tag>(
    // 1. Configure Options (TaggingOptions)
    configureOptions => 
    {
        // Example: Set a custom separator for batch tag creation
        configureOptions.MaximumTagsPerEntity = 10; // By default, its 10, but you can change it here
    },
    // 2. Configure Database Context (DbContextOptionsBuilder)
    configureDbContext => 
    {
        // Example: Use SQLite for the tagging database. 
        // NOTE: This context is separate from your main application context.
        configureDbContext.UseSqlite(builder.Configuration.GetConnectionString("TaggingConnection"));
    }
);

// Add your main DbContext and other services...
builder.Services.AddControllers();
var app = builder.Build();

// ... rest of the app setup
```

### 3. Inject and Use the Repository

Once configured, you can inject the ITaggableRepository into your services or controllers to manage tags for your Product entity:
```csharp
public class ProductService(ITaggableRepository<Product, ProductTagJoin, Tag> tagRepository)
{
    private readonly ITaggableRepository<Product, ProductTagJoin, Tag> _tagRepository = tagRepository;
    
    public async Task AddTagsToProduct(Product myProduct, ICollection<Tag> tags)
    {
        // The repository handles the complex logic of parsing tags, 
        // creating new tags if they don't exist, and linking them to the entity.
        var result = await _tagRepository.AddTagToEntityAsync(myProduct, tags);

        // result contains metadata about created and attached tags
        Console.WriteLine($"Tags added: {string.Join(", ", result)}"); // Example output: Tags added: 4
    }
    
    public async Task<IEnumerable<Tag>> GetProductTags(Product myProduct)
    {
        return await _tagRepository.GetTagsForEntityAsync(myProduct);
    }
}
```

There is also a class called TagManager which provides methods for using the tagging system without needing to define custom models. This is useful for quick implementations or prototyping.
### 4. Using TagManager
```csharp
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
```

## Additional Usage Examples

You can add validation to tag before adding them to an entity, a default implementation is provided in the namespace
`Live.AndrewCrossan.Tagging.Validation`

Currently, only the following validation is provided:
- Ensuring the maximum number of tags per entity is not exceeded.
- Ensuring no duplicate tags are added to an entity.
- Ensuring tag names are not null or empty.
- Ensuring tags are alphanumeric.

This can be modified by implementing the `ITagValidator<TTag>` interface, like everything else in this library, it uses generics to allow for maximum flexibility. Here is an example of a custom tag validator:

```csharp
public class MyTagValidator : ITagValidator<Tag>
{
    public List<string> Errors { get; private set; } = new List<string>();

    public async Task<bool> ValidateAsync(Tag tag)
    {
        Errors.Clear();

        if (string.IsNullOrWhiteSpace(tag.Name))
        {
            Errors.Add("Tag name cannot be null or empty.");
        }

        if (!tag.Name.All(char.IsLetterOrDigit))
        {
            Errors.Add("Tag name must be alphanumeric.");
        }

        // Add more custom validation rules as needed

        return Errors.Count == 0;
    }
}
```

You can also extend the ITaggableRepository to add your own custom methods for more specific use cases. Here's an example of how to create a custom repository that extends the base functionality:

```csharp
public class CustomTaggableRepository : ITaggableRepository<Product, ProductTagJoin, Tag>
{
    private readonly ITaggableRepository<Product, ProductTagJoin, Tag> _baseRepository;

    public CustomTaggableRepository(ITaggableRepository<Product, ProductTagJoin, Tag> baseRepository)
    {
        _baseRepository = baseRepository;
    }

    public async Task<IEnumerable<Tag>> GetTagsByPrefixAsync(Product entity, string prefix)
    {
        var allTags = await _baseRepository.GetTagsForEntityAsync(entity);
        return allTags.Where(tag => tag.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    // Implement other methods from ITaggableRepository by delegating to _baseRepository
}
```

You can also extend the ITaggableEntity to add more properties to the join table. Here's an example of how to create a custom join entity that includes a timestamp for when the tag was added:

```csharp
public class ProductTagJoin : ITaggableEntity<Product, Tag>
{
    // Required foreign keys
    public Guid EntityId { get; set; }
    public Guid TagId { get; set; }

    // Required navigation properties
    public Product Entity { get; set; }
    public Tag Tag { get; set; }

    // Custom property to track when the tag was added
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
```

You can also use the built-in Tag class if you don't need a custom tag model. Here's an example of how to use the built-in Tag class:

```csharp
// In Program.cs use this instead of the previous example
builder.Services.AddTagging<Product, ProductTagJoin>(
    // 1. Configure Options (TaggingOptions)
    configureOptions => 
    {
        // Example: Set a custom separator for batch tag creation
        configureOptions.MaximumTagsPerEntity = 10; // By default, its 10, but you can change it here
    },
    // 2. Configure Database Context (DbContextOptionsBuilder)
    configureDbContext => 
    {
        // Example: Use SQLite for the tagging database. 
        // NOTE: This context is separate from your main application context.
        configureDbContext.UseSqlite(builder.Configuration.GetConnectionString("TaggingConnection"));
    }
);
```

The Default Tag Model;
```csharp
public class Tag : BaseTag
{
    [MaxLength(100)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Slug => Name?.ToLower().Replace(" ", "-") ?? string.Empty;
}
```

## Logging
The library includes built-in logging to help you monitor and debug the tagging operations. You can configure the logging level and output format using the standard .NET logging configuration.

Shutting the logger up is as simple as adding the logging services in appsettings.json or in code.
```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Warning",
            "Live.AndrewCrossan.Tagging": "Error"
        }
    }
}
```

### Logging Messages

The entirety of the logging messages can be found here: [Tagging Logging Messages](Logging/TaggingLogMessages.cs)

## Contact
### 📧 Email

Andrew Crossan – andrew.crossan23@outlook.com<br>
Project Link: [Tagging](https://github.com/AndrewCrossan1/Tagging)

### LinkedIn
Connect with me on [LinkedIn](https://www.linkedin.com/in/andrewcrossan1/)