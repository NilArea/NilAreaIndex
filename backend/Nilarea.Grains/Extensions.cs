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
            return modelBuilder;
        }
    }
}
