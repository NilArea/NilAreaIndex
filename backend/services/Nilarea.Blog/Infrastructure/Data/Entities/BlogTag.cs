namespace NilArea.Blog.Infrastructure.Data.Entities;

public sealed class BlogTag
{
    public required int Id { get; init; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
}