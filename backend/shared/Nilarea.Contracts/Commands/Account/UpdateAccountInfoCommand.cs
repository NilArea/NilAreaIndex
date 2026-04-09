using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.UpdateAccountInfoCommand")]
public record UpdateAccountInfoCommand
{
    [Id(0)] public required Guid UserId { get; init; }
    [Id(1)] public string? Username { get; init; }
    [Id(2)] public string? Email { get; init; }
}

internal sealed class UpdateAccountInfoValidator : AbstractValidator<UpdateAccountInfoCommand>
{
    public UpdateAccountInfoValidator()
    {
        RuleFor(r => r.UserId)
            .NotEmpty().WithMessage("User ID is required.");
        RuleFor(r => r.Username)
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");
        RuleFor(r => r.Email)
            .EmailAddress().WithMessage("Invalid email address.")
            .When(r => !string.IsNullOrEmpty(r.Email));
    }
}