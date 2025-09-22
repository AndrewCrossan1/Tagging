using Live.AndrewCrossan.Tagging.Context;
using Live.AndrewCrossan.Tagging.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Live.AndrewCrossan.Tagging.Tests.ModelsTest;

public class PostTag : ITaggableEntity<Post, Tag>
{
    public Guid EntityId { get; set; }
    public Post Entity { get; set; }
    public Guid TagId { get; set; }
    public Tag Tag { get; set; }
}

public class Post : BaseEntity<PostTag>, ITaggableModel
{
    public string Title { get; set;}
    public string Content { get; set; }
}

[TestFixture]
public class CanBeTagged
{
    
    private PostsApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<PostsApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new PostsApplicationDbContext(options);
    }
    
    // The DbContext for your application, which uses your generic library
    public class PostsApplicationDbContext : TaggingDbContext<Post, PostTag, Tag>
    {
        public PostsApplicationDbContext(DbContextOptions<PostsApplicationDbContext> options) : base(options) { }
        
        public DbSet<Post> Posts { get; set; }
    }
    
    [Test]
    public async Task Post_Can_Be_Tagged()
    {
        // Arrange
        await using var dbContext = GetInMemoryDbContext();
        
        var newPost = new Post { Title = "My First Post", Content = "Test content" };
        var newTag = new Tag { Name = "test-tag", Slug = "test-tag" };

        dbContext.Posts.Add(newPost);
        await dbContext.Tags.AddAsync(newTag);
        await dbContext.SaveChangesAsync();

        PostTag postTag = new PostTag { Entity = newPost, EntityId = newPost.Id, Tag = newTag, TagId = newTag.Id};
        await dbContext.TaggableEntities.AddAsync(postTag);
        await dbContext.SaveChangesAsync();

        // Act
        var retrievedPost = await dbContext.Posts
            .Include(p => p.Tags)
            .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == newPost.Id);
        
        // Assert
        Assert.That(retrievedPost, Is.Not.Null);
        Assert.That(retrievedPost.Tags, Is.Not.Null);
        Assert.That(retrievedPost.Tags.Count, Is.EqualTo(1));
        Assert.That(retrievedPost.Tags.First().Tag.Name, Is.EqualTo(newTag.Name));
    }
}