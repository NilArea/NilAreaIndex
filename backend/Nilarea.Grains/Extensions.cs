using Microsoft.EntityFrameworkCore;
using NilArea.Grains.Dtos;

namespace NilArea.Grains;

public static class Extensions
{
    extension(ModelBuilder modelBuilder)
    {
        public ModelBuilder ApplyNilareaContractsConfig()
        {
            var ec = new AccountUserEntityConfig();
            modelBuilder
                .ApplyConfiguration<AccountUserDto>(ec)
                .ApplyConfiguration<AccountGroupDto>(ec)
                .ApplyConfiguration<AccountUserGroup>(ec);
            modelBuilder.Entity<AccountUserDto>()
                .HasQueryFilter(static u => u.DeleteAt == null);
            modelBuilder.Entity<AccountGroupDto>()
                .HasQueryFilter(static g => g.DeleteAt == null);
            return modelBuilder;
        }
    }
}
