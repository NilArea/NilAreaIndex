namespace NilArea.Contracts.Responses.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Responses.Account.GroupInfoResponse")]
public class GroupInfoResponse
{
    [Id(0)] public required int GroupId { get; set; }
    [Id(1)] public required string GroupName { get; set; }
    [Id(2)] public required string Description { get; set; }
    [Id(3)] public required bool IsSystemGroup { get; set; }
    [Id(4)] public required DateTime CreatedAt { get; set; }
    [Id(5)] public required DateTime UpdatedAt { get; set; }
    [Id(6)] public required DateTime? DeletedAt { get; set; }
}