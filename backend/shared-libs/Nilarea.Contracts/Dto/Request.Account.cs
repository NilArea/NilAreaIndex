namespace NilArea.Contracts.Dto;

public static class Requests
{
    [GenerateSerializer]
    [Alias("NilArea.Contracts.Dto.Requests.RegisterAccount")]
    public class RegisterAccount
    {
        [Id(0)] public required string Email { get; set; }
        [Id(1)] public required string Password { get; set; }
        [Id(2)] public required string Username { get; set; }
        [Id(3)] public required string ConfirmKey { get; set; }
    }

    [GenerateSerializer]
    [Alias("NilArea.Contracts.Dto.Requests.LoginAccount")]
    public class LoginAccount
    {
        [Id(0)] public required string Email { get; set; }
        [Id(1)] public required string Password { get; set; }
    }

    [GenerateSerializer]
    [Alias("NilArea.Contracts.Dto.Requests.DeleteAccount")]
    public class DeleteAccount
    {
        [Id(0)] public required string Email { get; set; }
        [Id(2)] public required string Password { get; set; }
        [Id(1)] public required string ConfirmKey { get; set; }
    }

    [GenerateSerializer]
    [Alias("NilArea.Contracts.Dto.Requests.ChangePasswd")]
    public class ChangePasswd
    {
        [Id(0)] public required string Email { get; set; }
        [Id(2)] public required string Password { get; set; }
        [Id(3)] public required string NewPassword { get; set; }
        [Id(1)] public required string ConfirmKey { get; set; }
    }
}
