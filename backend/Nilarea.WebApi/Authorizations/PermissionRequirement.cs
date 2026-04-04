using Microsoft.AspNetCore.Authorization;

namespace NilArea.Web.Authorizations;

public class PermissionRequirement(params IEnumerable<string> permissionTags) : IAuthorizationRequirement
{
    public ICollection<string> RequiredTags { get; } = permissionTags.ToHashSet();
}