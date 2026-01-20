using Microsoft.EntityFrameworkCore;
using NilArea.Grains.Dtos;

namespace NilArea.Grains;

public static class Extensions
{
    extension(ModelBuilder modelBuilder)
    {
        public ModelBuilder ApplyNilareaContractsConfig()
        {
            return modelBuilder.ApplyConfiguration(new AccountUserEntityConfig());
        }
    }
}
