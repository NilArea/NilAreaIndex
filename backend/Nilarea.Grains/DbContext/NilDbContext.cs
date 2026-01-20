using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NilArea.Common.Utils;
using NilArea.Grains.Dtos;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace NilArea.Grains.DbContext;

public class NilDbContext(IConfiguration configuration) : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<AccountDbDto> AccountUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        var sqlConnectionString = configuration.SafeGetConfigureValue("MYSQL_CONNECTION_STRING");
        var sqlVersion = new MySqlServerVersion(new Version(9, 5));
        optionsBuilder.UseMySql(sqlConnectionString, sqlVersion, o => o.SchemaBehavior(MySqlSchemaBehavior.Ignore));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyNilareaContractsConfig();
    }
}
