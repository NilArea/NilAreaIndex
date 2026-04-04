using FluentValidation;
using NilArea.Common;
using NilArea.Contracts;

namespace NilArea.Web.Configure;

internal static partial class Configure
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureDataValidator()
        {
            builder.Services
                .AddCommonValidators()
                .AddContractsValidators()
                .AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
            return builder;
        }
    }
}