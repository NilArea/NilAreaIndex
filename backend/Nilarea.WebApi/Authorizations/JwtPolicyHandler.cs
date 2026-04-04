using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using NilArea.Contracts;
using NilArea.Contracts.Grains.Account;

namespace NilArea.Web.Authorizations;

public class JwtPolicyHandler(
    ILogger<JwtPolicyHandler> logger,
    IClusterClient clusterClient
) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var ssot = clusterClient.GetGrain<IAuthenticationGrain>(Guid.Empty);
        try
        {
            if (await ssot.VarifyPermissionAsync(Guid.CreateVersion7(), [StaticValues.PermissionTags.SystemAdmin]))
                context.Succeed(requirement);
        }
        catch (AuthenticationException)
        {
        }

        return;
        var httpCtx = (DefaultHttpContext)context.Resource!;
        var at = httpCtx.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "");

        // 无 token 直接失败
        if (string.IsNullOrEmpty(at)) return;

        // 验签 + 读 claim
        var principal = ValidateToken(at);
        if (principal is null) return;
        var userId = Guid.Parse(principal.FindFirst("uid")!.Value);

        // 实时拿 tags
        var sso = clusterClient.GetGrain<IAuthenticationGrain>(Guid.Empty);
        try
        {
            if (await sso.VarifyPermissionAsync(userId, requirement.RequiredTags))
                context.Succeed(requirement);
        }
        catch (AuthenticationException)
        {
        }
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            return handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey("your-256-bit-secret"u8.ToArray()),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);
        }
        catch
        {
            return null;
        }
    }
}