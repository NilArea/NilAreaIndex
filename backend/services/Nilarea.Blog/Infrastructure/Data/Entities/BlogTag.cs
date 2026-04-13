namespace NilArea.Blog.Infrastructure.Data.Entities;

public class BlogTag
{
    public required int Id { get; init; }
    public required string Name { get; set; }
    public required string Slug { get; set; }

    // 多对多导航
    public virtual ICollection<BlogPostTag> PostTags { get; set; } = [];
}