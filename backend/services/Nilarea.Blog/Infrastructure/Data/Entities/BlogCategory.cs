namespace NilArea.Blog.Infrastructure.Data.Entities;

/// <summary>
///     文章分类
/// </summary>
public class BlogCategory
{
    public required int Id { get; init; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public required int? ParentCategoryId { get; set; }
    public required DateTime CreatedAt { get; init; }

    // 导航属性
    public virtual BlogCategory? ParentCategory { get; set; }
    public virtual ICollection<BlogCategory> ChildCategories { get; set; } = [];
    public virtual ICollection<BlogPost> Posts { get; set; } = [];
}