namespace NilArea.Blog.Infrastructure.Data.Entities;

public sealed class BlogPost
{
    public required long Id { get; init; }
    public required string Slug { get; set; }

    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Content { get; set; }

    public required DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int Version { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public required long AuthorId { get; init; }
}