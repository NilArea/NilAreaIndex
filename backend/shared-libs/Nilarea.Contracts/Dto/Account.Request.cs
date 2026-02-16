namespace NilArea.Contracts.Dto;

[GenerateSerializer]
public class DeleteAccountRequest
{
    [Id(0)] public required string Email { get; set; }
    [Id(2)] public required string Password { get; set; }
    [Id(1)] public required string ConfirmKey { get; set; }
}

[GenerateSerializer]
public class ChangePasswdRequest
{
    [Id(0)] public required string Email { get; set; }
    [Id(2)] public required string Password { get; set; }
    [Id(3)] public required string NewPassword { get; set; }
    [Id(1)] public required string ConfirmKey { get; set; }
}

[GenerateSerializer]
public class AccountLoginRequest
{
    [Id(0)] public required string Email { get; set; }
    [Id(1)] public required string Password { get; set; }
}

[GenerateSerializer]
public class AccountRegisterRequest
{
    [Id(0)] public required string Email { get; set; }
    [Id(1)] public required string Password { get; set; }
    [Id(2)] public required string Username { get; set; }
    [Id(3)] public required string ConfirmKey { get; set; }
}
