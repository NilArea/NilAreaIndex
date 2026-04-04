namespace NilArea.Contracts.Responses.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Responses.Account.LoginAccountResponse")]
public class LoginAccountResponse
{
    [Id(0)] public required Guid UserId { get; set; }
    [Id(1)] public required string AccessToken { get; set; }
    [Id(2)] public required DateTime AccessTokenExpiry { get; set; }
    [Id(3)] public required string RefreshToken { get; set; }
    [Id(4)] public required DateTime RefreshTokenExpiry { get; set; }
}