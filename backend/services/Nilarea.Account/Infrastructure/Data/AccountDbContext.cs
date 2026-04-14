using Microsoft.EntityFrameworkCore;
using NilArea.Account.Infrastructure.Data.Entities;
using NilArea.Account.Infrastructure.Data.EntityConfigurations;

namespace NilArea.Account.Infrastructure.Data;

public class
    AccountDbContext(DbContextOptions<AccountDbContext> options) : DbContext(options)
{
    public DbSet<AccountUser> AccountUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder
            .ApplyConfiguration(new AccountUserConfiguration());
    }
}