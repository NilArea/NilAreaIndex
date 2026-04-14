namespace NilArea.Blog.Infrastructure.Data.Entities;

/// <summary>
///     文章分类
/// </summary>
public sealed class BlogCategory
{
    public required int Id { get; init; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public required int? ParentCategoryId { get; set; }
    public required DateTimeOffset CreatedAt { get; init; }
}