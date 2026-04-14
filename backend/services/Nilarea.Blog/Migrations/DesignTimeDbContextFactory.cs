using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NilArea.Blog.Infrastructure.Data;

namespace NilArea.Blog.Migrations;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
{
    public BlogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BlogDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
        if (connectionString is null)
        {
            var path = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING_FILE");
            if (File.Exists(path)) connectionString = File.ReadAllText(path);
        }

        if (connectionString is null)
            throw new ArgumentNullException(nameof(connectionString));
        optionsBuilder.UseMySQL(connectionString,
            option => option.MigrationsHistoryTable($"__EFMigrationsHistory_{nameof(Blog)}"));
        return new BlogDbContext(optionsBuilder.Options);
    }
}