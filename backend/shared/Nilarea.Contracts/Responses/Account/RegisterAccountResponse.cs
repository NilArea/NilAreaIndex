namespace NilArea.Contracts.Responses.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Responses.Account.RegisterAccountResponse")]
public sealed record RegisterAccountResponse
{
    [Id(0)] public required long UserId { get; init; }
    [Id(1)] public required string Email { get; init; }
    [Id(2)] public required string Username { get; init; }
    [Id(3)] public required DateTimeOffset CreatedAt { get; init; }
}