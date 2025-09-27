using Live.AndrewCrossan.Tagging.Logging;
using Live.AndrewCrossan.Tagging.Managers;
using Live.AndrewCrossan.Tagging.Models;
using Live.AndrewCrossan.Tagging.Options;
using Live.AndrewCrossan.Tagging.Repositories;
using Live.AndrewCrossan.Tagging.Validation;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Live.AndrewCrossan.Tagging.Tests.Managers;

[TestFixture]
public class TagManagerTests
{
    private Mock<ITagRepository<Tag>> _tagRepository;
    private Mock<TaggingOptions> _options;
    private Mock<ITagValidator<Tag>> _validator;
    private Mock<ILogger<TagManager<Tag>>> _logger;
    private TagManager<Tag> _tagManager;

    [SetUp]
    public async Task SetUp()
    {
        // Create mock DbContext
        _tagRepository = new Mock<ITagRepository<Tag>>();
        _options = new Mock<TaggingOptions>();
        _validator = new Mock<ITagValidator<Tag>>();
        _logger = new Mock<ILogger<TagManager<Tag>>>();
        _tagManager = new TagManager<Tag>(_logger.Object, _tagRepository.Object, _validator.Object, _options.Object);
    }

    [Test]
    public async Task CreateOrGetTagsAsync_validation_okay()
    {
        // Arrange
        var tags = new List<Tag>();
        List<Guid> ids = new List<Guid>();

        for (int i = 0; i < 3; i++)
        {
            tags.Add(new()
            {
                Name = "Tag #" + i,
                Description = "Description for Tag #" + i
            });
            
            ids.Add(tags[i].Id);    
        }
        
        _tagRepository.Setup(r => r.GetTagsByNamesAsync(It.IsAny<IEnumerable<string>>(), CancellationToken.None))
            .ReturnsAsync(new List<Tag> { tags[1] }); // Simulate that the second tag already exists
        _validator.Setup(v => v.ValidateAsync(It.IsAny<Tag>(), CancellationToken.None))
            .ReturnsAsync((Tag tag, CancellationToken ct) => true); // No validation errors
        _tagRepository.Setup(r => r.SaveRangeAsync(It.IsAny<IEnumerable<Tag>>(), CancellationToken.None));
        
        // Act
        var result = await _tagManager.CreateOrGetTagsAsync(tags);
        
        // Assert
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Single(t => t.Name == tags[0].Name).Name, Is.EqualTo(tags[0].Name));
        Assert.That(result.Single(t => t.Name == tags[1].Name).Name, Is.EqualTo(tags[1].Name));
        Assert.That(result.Single(t => t.Name == tags[2].Name).Name, Is.EqualTo(tags[2].Name));

        var names = tags.Select(tag => tag.Name);
        
        _tagRepository.Verify(r => r.GetTagsByNamesAsync(names, CancellationToken.None), Times.Once);
    }
    
    [Test]
    public async Task CreateOrGetTagsAsync_validation_error()
    {
        // Arrange
        var tags = new List<Tag>();
        List<Guid> ids = new List<Guid>();

        for (int i = 0; i < 3; i++)
        {
            tags.Add(new()
            {
                Name = "Tag #" + i,
                Description = "Description for Tag #" + i
            });
            
            ids.Add(tags[i].Id);    
        }

        tags[0].Name = "";
        
        _tagRepository.Setup(r => r.GetTagsByNamesAsync(It.IsAny<IEnumerable<string>>(), CancellationToken.None))
            .ReturnsAsync(new List<Tag> { tags[1] }); // Simulate that the second tag already exists
        _validator.Setup(v => v.ValidateAsync(tags[0], CancellationToken.None))
            .ReturnsAsync(false); // Validation Error on tags[0]
        _tagRepository.Setup(r => r.SaveRangeAsync(It.IsAny<IEnumerable<Tag>>(), CancellationToken.None));
        
        // Act
        Assert.ThrowsAsync<TagValidationException>(async () => await _tagManager.CreateOrGetTagsAsync(tags));
        Assert.That(_logger.Invocations.Count, Is.EqualTo(1));
    }
    
    [Test]
    public async Task SaveTagAsync() {}
    
    [Test]
    public async Task DeleteTagAsync() {}
    
    [Test]
    public async Task GetAllTagsAsync() {}
    
    [Test]
    public async Task GetTagByIdAsync() {}
}