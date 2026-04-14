namespace NilArea.Contracts.Responses.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Responses.Account.AccountInfoResponse")]
public sealed record AccountInfoResponse
{
    [Id(0)] public required long UserId { get; init; }
    [Id(1)] public required string Email { get; init; }
    [Id(2)] public required string Username { get; init; }
    [Id(3)] public required DateTimeOffset CreatedAt { get; init; }
    [Id(4)] public required DateTimeOffset UpdatedAt { get; init; }
}