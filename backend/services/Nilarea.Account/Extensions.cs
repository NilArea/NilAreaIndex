using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace NilArea.Account;

public static class Extensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddAccountValidator()
        {
            return collection
                .AddValidatorsFromAssembly(typeof(Extensions).Assembly);
        }
    }
}