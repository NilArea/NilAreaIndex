using Microsoft.Extensions.Logging;
using NilArea.Blog.States;
using NilArea.Contracts.Grains.Blog;

namespace NilArea.Blog.Grains;

public class BlogGrain(
    ILogger<BlogGrain> logger
) : Grain<BlogStates>, IBlogGrain
{
    public async ValueTask PingAsync()
    {
        logger.LogInformation("PingAsync");
    }
}