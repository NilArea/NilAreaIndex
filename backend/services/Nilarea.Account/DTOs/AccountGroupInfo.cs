namespace NilArea.Account.DTOs;

public class AccountGroupInfo
{
    public required int GroupId { get; init; }
    public required string GroupName { get; init; }
    public required string Description { get; set; }
    public required bool IsSystemGroup { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; set; }
    public required DateTime? DeletedAt { get; set; }
}