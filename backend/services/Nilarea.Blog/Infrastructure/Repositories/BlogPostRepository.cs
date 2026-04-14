using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NilArea.Blog.Infrastructure.Data;

namespace NilArea.Blog.Infrastructure.Repositories;

public class BlogPostRepository(
    ILogger<BlogPostRepository> logger,
    IDbContextFactory<BlogDbContext> dbContextFactory
) : IBlogPostRepository
{
}