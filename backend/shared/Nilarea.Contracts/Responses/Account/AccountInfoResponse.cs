namespace NilArea.Contracts.Responses.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Responses.Account.AccountInfoResponse")]
public record AccountInfoResponse
{
    [Id(0)] public required Guid UserId { get; init; }
    [Id(1)] public required string Email { get; init; }
    [Id(2)] public required string Username { get; init; }
    [Id(3)] public required DateTime CreatedAt { get; init; }
}