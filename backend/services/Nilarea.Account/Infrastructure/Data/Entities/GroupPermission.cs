namespace NilArea.Account.Infrastructure.Data.Entities;

public class GroupPermission
{
    public required int GroupId { get; init; }
    public required short PermissionId { get; init; }
    public virtual AccountGroup Group { get; set; } = null!;
    public virtual PermissionTag Permission { get; set; } = null!;
}