namespace NilArea.Contracts.Dto;

[GenerateSerializer]
public class TokenPair
{
    [Id(0)] public required string AccessToken { get; init; }
    [Id(1)] public required DateTime AccessExpire { get; init; }
    [Id(2)] public required string RefreshToken { get; init; }
    [Id(3)] public required DateTime RefreshExpire { get; init; }
}
