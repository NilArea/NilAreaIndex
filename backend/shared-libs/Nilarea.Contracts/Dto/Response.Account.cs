namespace NilArea.Contracts.Dto;

[GenerateSerializer]
public class AccountLoginResponse
{
    [Id(0)] public required Guid UserId { get; set; }
    [Id(1)] public required string AccessToken { get; set; }
    [Id(2)] public required DateTime AccessTokenExpiry { get; set; }
    [Id(3)] public required string RefreshToken { get; set; }
    [Id(4)] public required DateTime RefreshTokenExpiry { get; set; }
}

[GenerateSerializer]
public class AccountRegisterResponse
{
    [Id(0)] public required Guid UserId { get; set; }
    [Id(1)] public required string Email { get; set; }
    [Id(2)] public required string Username { get; set; }
    [Id(3)] public required DateTime CreatedAt { get; set; }
}
