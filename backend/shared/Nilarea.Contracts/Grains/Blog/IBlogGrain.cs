namespace NilArea.Contracts.Grains.Blog;

public interface IBlogGrain : IGrainWithGuidKey
{
    [Alias("PingAsync")]
    ValueTask PingAsync();
}