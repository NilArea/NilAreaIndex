namespace NilArea.Blog.Infrastructure.Data.Entities;

public class BlogPost
{
    public required int Id { get; init; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Content { get; set; }
    public BlogPostStatus Status { get; set; }
    public int ViewCount { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public required Guid AuthorId { get; init; }
    public required int CategoryId { get; set; }

    // 导航属性
    public virtual BlogCategory Category { get; set; } = null!;
    public virtual ICollection<Comment> Comments { get; set; } = [];
    public virtual ICollection<BlogPostTag> PostTags { get; set; } = [];
}

/// <summary>
///     文章状态
/// </summary>
public enum BlogPostStatus
{
    Draft, // 草稿
    Published, // 已发布
    Archived // 已归档
}