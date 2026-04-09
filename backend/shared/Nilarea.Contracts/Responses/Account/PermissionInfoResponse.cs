namespace NilArea.Contracts.Responses.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Responses.Account.PermissionInfoResponse")]
public class PermissionInfoResponse
{
    [Id(0)] public required short PermissionId { get; set; }
    [Id(1)] public required string PermissionName { get; set; }
    [Id(2)] public required string Description { get; set; }
    [Id(3)] public required DateTime CreatedAt { get; set; }
    [Id(4)] public required DateTime? UpdatedAt { get; set; }
}