using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NilArea.Account.Infrastructure.Data;

namespace NilArea.Account.Migrations;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
{
    public AccountDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
        if (connectionString is null)
        {
            var path = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING_FILE");
            if (File.Exists(path)) connectionString = File.ReadAllText(path);
        }

        if (connectionString is null)
            throw new ArgumentNullException(nameof(connectionString));
        optionsBuilder.UseMySQL(connectionString,
            option => option.MigrationsHistoryTable($"__EFMigrationsHistory_{nameof(Account)}"));
        return new AccountDbContext(optionsBuilder.Options);
    }
}