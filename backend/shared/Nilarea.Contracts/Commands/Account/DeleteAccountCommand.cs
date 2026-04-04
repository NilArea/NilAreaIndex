namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.DeleteAccountCommand")]
public class DeleteAccountCommand
{
    [Id(0)] public required string Email { get; set; }
    [Id(2)] public required string Password { get; set; }
    [Id(1)] public required string ConfirmKey { get; set; }
}