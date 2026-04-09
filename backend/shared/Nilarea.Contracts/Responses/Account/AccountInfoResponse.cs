namespace NilArea.Contracts.Responses.Account;

public record AccountInfoResponse
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string Username { get; init; }
    public required DateTime CreatedAt { get; init; }
}