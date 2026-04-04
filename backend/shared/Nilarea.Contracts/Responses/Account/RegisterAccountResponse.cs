namespace NilArea.Contracts.Responses.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Responses.Account.RegisterAccountResponse")]
public class RegisterAccountResponse
{
    [Id(0)] public required Guid UserId { get; set; }
    [Id(1)] public required string Email { get; set; }
    [Id(2)] public required string Username { get; set; }
    [Id(3)] public required DateTime CreatedAt { get; set; }
}