using FluentValidation;
using NilArea.Common;
using NilArea.Contracts;
using NilArea.Interfaces;

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
                .AddInterfaceValidators()
                .AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
            return builder;
        }
    }
}
