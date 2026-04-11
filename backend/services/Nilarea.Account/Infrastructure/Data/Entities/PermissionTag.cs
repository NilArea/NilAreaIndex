using System.ComponentModel.DataAnnotations;

namespace NilArea.Account.Infrastructure.Data.Entities;

public class PermissionTag
{
    public short PermissionId { get; init; }
    [MaxLength(100)] public required string PermissionName { get; init; }
    [MaxLength(500)] public required string Description { get; set; }
    [DataType("datetime(6)")] public required DateTime CreatedAt { get; init; }
    [DataType("datetime(6)")] public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<UserPermission> UserPermissions { get; } = [];
    public virtual ICollection<GroupPermission> GroupPermissions { get; } = [];
}