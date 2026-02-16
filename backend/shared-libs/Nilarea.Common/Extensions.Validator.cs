using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace NilArea.Common;

public static partial class Extensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddCommonValidators()
        {
            return collection
                .AddValidatorsFromAssembly(typeof(Extensions).Assembly, includeInternalTypes: true);
        }
    }
}
