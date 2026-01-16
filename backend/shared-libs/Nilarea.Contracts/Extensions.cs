using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NilArea.Contracts.Validators;

namespace NilArea.Contracts;

public static class Extensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddContractsValidators()
        {
            return collection
                .AddValidatorsFromAssemblyContaining<RegisterRequestValidator>(includeInternalTypes: true);
        }
    }
}
