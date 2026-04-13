namespace NilArea.Blog.Infrastructure.Data.Entities;

public class BlogPostTag
{
    public required int PostId { get; init; }
    public required int TagId { get; init; }

    public virtual BlogPost Post { get; set; } = null!;
    public virtual BlogTag Tag { get; set; } = null!;
}