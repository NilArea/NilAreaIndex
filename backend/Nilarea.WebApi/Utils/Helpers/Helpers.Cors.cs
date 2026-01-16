using Microsoft.AspNetCore.Cors.Infrastructure;

namespace NilArea.Api.Utils.Helpers;

internal static partial class Helpers
{
    public static void CorsSetup(CorsOptions options)
    {
        options.AddPolicy(name: "_myAllowSpecificOrigins", policy => { });
    }
}
