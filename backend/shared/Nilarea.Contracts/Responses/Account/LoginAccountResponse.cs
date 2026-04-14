namespace NilArea.Contracts.Responses.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Responses.Account.LoginAccountResponse")]
public sealed record LoginAccountResponse
{
    [Id(0)] public required long UserId { get; init; }
    [Id(1)] public required string AccessToken { get; init; }
    [Id(2)] public required DateTimeOffset AccessTokenExpiry { get; init; }
    [Id(3)] public required string RefreshToken { get; init; }
    [Id(4)] public required DateTimeOffset RefreshTokenExpiry { get; init; }
}