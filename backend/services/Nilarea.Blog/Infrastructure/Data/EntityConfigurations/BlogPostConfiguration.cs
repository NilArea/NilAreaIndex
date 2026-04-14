using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NilArea.Blog.Infrastructure.Data.Entities;

namespace NilArea.Blog.Infrastructure.Data.EntityConfigurations;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("BlogPosts");
        builder.HasKey(e => e.Id)
            .HasName("PK_BlogPosts_PostId");

        builder.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 字段 ---------- */
        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(e => e.Description)
            .IsRequired();
        builder.Property(e => e.Content)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .HasColumnType("datetime(6)")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAdd();
        builder.Property(e => e.UpdatedAt)
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAddOrUpdate();
        builder.Property(e => e.DeletedAt)
            .HasColumnType("datetime(6)")
            .ValueGeneratedNever();
        builder.Property(e => e.Version)
            .IsRequired()
            .HasDefaultValueSql("0");
        builder.Property(e => e.AuthorId)
            .IsRequired()
            .ValueGeneratedNever();
        /* ---------- 索引 ---------- */
        builder.HasIndex(e => new { e.Slug, e.DeletedAt })
            .HasDatabaseName("IX_BlogPost_Slug_DeletedAt")
            .IsUnique();
        builder.HasIndex(e => new { e.AuthorId, e.CreatedAt })
            .HasDatabaseName("IX_BlogPost_AuthorId_CreatedAt");
        builder.HasIndex(e => e.UpdatedAt)
            .HasDatabaseName("IX_BlogPost_UpdatedAt");
    }
}