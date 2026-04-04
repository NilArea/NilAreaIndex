namespace NilArea.Account.Infrastructure.Data.Entities;

public class UserPermission
{
    public required Guid UserId { get; init; }
    public required short PermissionId { get; init; }
    public virtual AccountUser User { get; set; } = null!;
    public virtual PermissionTag Permission { get; set; } = null!;
}