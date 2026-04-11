namespace NilArea.Account.DTOs;

public record AccountUserInfo
{
    public required Guid UserId { get; init; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public required DateTime CreatedAt { get; init; }
    public IEnumerable<string>? Groups { get; set; }
}

public record PermissionTagInfo(
    short PermissionId,
    string PermissionName);