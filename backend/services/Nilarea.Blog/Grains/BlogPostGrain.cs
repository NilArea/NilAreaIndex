using Microsoft.Extensions.Logging;
using NilArea.Blog.States;
using NilArea.Contracts.Grains.Blog;

namespace NilArea.Blog.Grains;

public class BlogPostGrain(
    ILogger<BlogPostGrain> logger
) : Grain<BlogPostMetadata>, IBlogPostGrain
{
}