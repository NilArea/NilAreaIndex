namespace NilArea.Blog.Infrastructure.Data.Entities;

public sealed class BlogPost
{
    public required int Id { get; init; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Content { get; set; }
    public BlogPostStatus Status { get; set; }
    public required DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public required long AuthorId { get; init; }
    public required int CategoryId { get; set; }
}

/// <summary>
///     文章状态
/// </summary>
public enum BlogPostStatus
{
    /// 草稿
    Draft,

    /// 已发布
    Published,

    /// 已归档
    Archived
}