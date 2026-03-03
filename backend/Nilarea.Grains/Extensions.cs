using Microsoft.Extensions.DependencyInjection;

namespace NilArea.Grains;

public static class Extensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddNilareaValidator()
        {
            return collection;
        }
    }
}
