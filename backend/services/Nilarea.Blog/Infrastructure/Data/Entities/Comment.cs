namespace NilArea.Blog.Infrastructure.Data.Entities;

public sealed class Comment
{
    public required int Id { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? DeletedAt { get; set; }

    // 外键
    public required long AuthorId { get; set; }
    public required int PostId { get; set; }
    public int? ParentCommentId { get; set; }
}