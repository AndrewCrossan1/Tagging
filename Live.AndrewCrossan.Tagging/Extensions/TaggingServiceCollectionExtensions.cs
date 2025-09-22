using Live.AndrewCrossan.Tagging.Context;
using Live.AndrewCrossan.Tagging.Models;
using Live.AndrewCrossan.Tagging.Options;
using Live.AndrewCrossan.Tagging.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Live.AndrewCrossan.Tagging.Extensions;

public static class TaggingServiceCollectionExtensions
{
    /// <summary>
    /// Registers the tagging services with the specified DbContext configuration and tag model.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <param name="configureDbContext"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TJoin"></typeparam>
    /// <typeparam name="TTag"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddTagging<TEntity, TJoin, TTag>(
        this IServiceCollection services, Action<TaggingOptions> configureOptions, 
        Action<DbContextOptionsBuilder> configureDbContext)
        where TEntity : class, ITaggableModel
        where TJoin : class, ITaggableEntity<TEntity, TTag>, new()
        where TTag : class, ITag, new()
    {
        services.Configure(configureOptions);
        
        services.AddDbContext<TaggingDbContext<TEntity, TJoin, TTag>>(configureDbContext);
        
        // Register the tagging service
        services.TryAddScoped<ITaggableRepository<TEntity, TJoin, TTag>, TaggableRepository<TEntity, TJoin, TTag>>();
        services.TryAddScoped<ITagRepository<TEntity, TJoin, TTag>, TagRepository<TEntity, TJoin, TTag>>();
        
        return services;
    }

    /// <summary>
    /// Registers the tagging services with the specified DbContext configuration using the default Tag model.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <param name="configureDbContext"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TJoin"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddTagging<TEntity, TJoin>(
        this IServiceCollection services, Action<TaggingOptions> configureOptions,
        Action<DbContextOptionsBuilder> configureDbContext)
        where TEntity : class, ITaggableModel
        where TJoin : class, ITaggableEntity<TEntity, Tag>, new()
    {
        // Register the tagging service
        services.TryAddScoped<ITaggableRepository<TEntity, TJoin, Tag>, TaggableRepository<TEntity, TJoin, Tag>>();
        services.TryAddScoped<ITagRepository<TEntity, TJoin, Tag>, TagRepository<TEntity, TJoin, Tag>>();
        
        return services.AddTagging<TEntity, TJoin, Tag>(configureOptions, configureDbContext);
    }
}