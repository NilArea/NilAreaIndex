namespace NilArea.Blog.Infrastructure.Data.Entities;

public class Comment
{
    public required int Id { get; init; }
    public required string Content { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? DeletedAt { get; set; }

    // 外键
    public required int AuthorId { get; set; }
    public required int PostId { get; set; }
    public int? ParentCommentId { get; set; }

    // 导航属性
    public virtual BlogPost Post { get; set; } = null!;
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> ChildComments { get; set; } = [];
}