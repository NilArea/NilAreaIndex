using Microsoft.AspNetCore.Authorization;
using NilArea.Contracts;

namespace NilArea.Web.Authorizations;

public static class Policies
{
    public const string SystemAdmin = "SystemAdmin";
    public const string BlogAdmin = "BlogAdmin";
    public const string UserNeed = "UserNeed";

    extension(AuthorizationBuilder builder)
    {
        public AuthorizationBuilder AddNilareaAuthorization(IConfiguration configuration)
        {
            builder.AddPolicy(SystemAdmin,
                p => p.AddRequirements(new PermissionRequirement(StaticValues.PermissionTags.SystemAdmin)));
            builder.AddPolicy(BlogAdmin, p => p.Requirements.Add(new PermissionRequirement()));
            builder.AddPolicy(UserNeed, p => p.Requirements.Add(new PermissionRequirement()));
            builder.Services.AddSingleton<IAuthorizationHandler, JwtPolicyHandler>();
            return builder;
        }
    }
}