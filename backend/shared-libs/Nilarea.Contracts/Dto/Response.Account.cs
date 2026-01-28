namespace NilArea.Contracts.Dto;

public static class Responses
{
    [GenerateSerializer]
    [Alias("NilArea.Contracts.Dto.Responses.Register")]
    public class Register
    {
        [Id(0)] public required long UserId { get; set; }
        [Id(1)] public required string Email { get; set; }
        [Id(2)] public required string Username { get; set; }
        [Id(3)] public required DateTime CreatedAt { get; set; }
    }

    [GenerateSerializer]
    [Alias("NilArea.Contracts.Dto.Responses.Login")]
    public class Login
    {
        [Id(0)] public required long UserId { get; set; }
        [Id(1)] public required string AccessToken { get; set; }
        [Id(2)] public required DateTime AccessTokenExpiry { get; set; }
        [Id(3)] public required string RefreshToken { get; set; }
        [Id(4)] public required DateTime RefreshTokenExpiry { get; set; }
    }
}
