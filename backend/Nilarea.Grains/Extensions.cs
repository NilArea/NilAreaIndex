using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NilArea.Grains.Dbe;

namespace NilArea.Grains;

public static class Extensions
{
    extension(ModelBuilder modelBuilder)
    {
        public ModelBuilder ApplyNilareaContractsConfig()
        {
            var ec = new AccountUserEntityConfig();
            modelBuilder
                .ApplyConfiguration<AccountUser>(ec)
                .ApplyConfiguration<AccountGroup>(ec)
                .ApplyConfiguration<AccountUserGroup>(ec)
                .ApplyConfiguration<PermissionTag>(ec)
                .ApplyConfiguration<UserPermission>(ec)
                .ApplyConfiguration<GroupPermission>(ec);
            return modelBuilder;
        }
    }

    extension(IServiceCollection collection)
    {
        public IServiceCollection AddNilareaValidator()
        {
            return collection;
        }
    }
}
