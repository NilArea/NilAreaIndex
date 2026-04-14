using NilArea.Contracts.Enums;

namespace NilArea.Blog.States;

[GenerateSerializer]
[Serializable]
[Alias("NilArea.Blog.States.BlogPostMetadata")]
public class BlogPostMetadata
{
    [Id(0)] public int ContentVersion { get; set; }
    [Id(1)] public BlogPostStatus Status { get; set; }
    [Id(2)] public DateTimeOffset LastModified { get; set; }
}