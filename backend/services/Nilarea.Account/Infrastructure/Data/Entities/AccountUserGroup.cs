using System.ComponentModel.DataAnnotations;

namespace NilArea.Account.Infrastructure.Data.Entities;

public class AccountUserGroup
{
    public required Guid UserId { get; init; }
    public required int GroupId { get; init; }
    [DataType("datetime(6)")] public DateTime JoinedAt { get; init; }

    // 导航属性
    public virtual AccountUser User { get; set; } = null!;
    public virtual AccountGroup Group { get; set; } = null!;
}