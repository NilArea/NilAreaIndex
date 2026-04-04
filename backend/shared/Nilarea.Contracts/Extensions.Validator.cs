using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace NilArea.Contracts;

public static partial class Extensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddContractsValidators()
        {
            return collection
                .AddValidatorsFromAssembly(typeof(Extensions).Assembly, includeInternalTypes: true);
        }
    }
}