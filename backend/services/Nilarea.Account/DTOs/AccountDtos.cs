namespace NilArea.Account.DTOs;

public record AccountUserInfo(
    Guid UserId,
    string Email,
    string UserName,
    DateTime CreatedAt,
    IEnumerable<string>? Groups = null);

public record PermissionTagInfo(
    short PermissionId,
    string PermissionName);