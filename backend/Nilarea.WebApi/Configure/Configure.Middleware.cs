using NilArea.Web.Utils.ExceptionHandler;

namespace NilArea.Web.Configure;

internal static partial class Configure
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureGlobalExceptionHandler()
        {
            builder.Services
                .AddProblemDetails()
                .AddExceptionHandler<ValidationExceptionHandler>()
                .AddExceptionHandler<OrleansExceptionHandler>()
                .AddExceptionHandler<GlobalExceptionHandler>();
            return builder;
        }

        public IHostApplicationBuilder ConfigureSecurity()
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("_myAllowSpecificOrigins",
                    policy => { policy.WithOrigins("http://localhost:5173").WithOrigins("http://localhost:15173"); });
            });
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorizationBuilder();
            return builder;
        }
    }
}