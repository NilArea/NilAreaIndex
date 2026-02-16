using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace NilArea.Interfaces;

public static class Extensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddInterfaceValidators()
        {
            return collection
                .AddValidatorsFromAssembly(typeof(Extensions).Assembly, includeInternalTypes: true);
        }
    }
}
