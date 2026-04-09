namespace NilArea.Account.DTOs;

public class AccountGroupInfo
{
    public required int GroupId { get; init; }
    public required string GroupName { get; init; }
    public required string Description { get; set; }
    public required bool IsSystemGroup { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public required DateTime? DeletedAt { get; set; }
}