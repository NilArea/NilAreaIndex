namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.ChangePasswordCommand")]
public class ChangePasswordCommand
{
    [Id(0)] public required string Email { get; set; }
    [Id(2)] public required string Password { get; set; }
    [Id(3)] public required string NewPassword { get; set; }
    [Id(1)] public required string ConfirmKey { get; set; }
}