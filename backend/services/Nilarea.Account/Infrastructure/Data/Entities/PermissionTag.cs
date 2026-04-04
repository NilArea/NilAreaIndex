using System.ComponentModel.DataAnnotations;

namespace NilArea.Account.Infrastructure.Data.Entities;

public class PermissionTag
{
    public required short PermissionId { get; init; }
    [MaxLength(100)] public required string PermissionName { get; init; }
    public virtual ICollection<UserPermission> UserPermissions { get; } = [];
    public virtual ICollection<GroupPermission> GroupPermissions { get; } = [];
}