using Microsoft.AspNetCore.Cors.Infrastructure;

namespace NilArea.Api.Utils.Helpers;

internal static partial class Helpers
{
    public static void CorsSetup(CorsOptions options)
    {
        options.AddPolicy("_myAllowSpecificOrigins", policy => { policy.WithOrigins("http://localhost:5173"); });
    }
}
