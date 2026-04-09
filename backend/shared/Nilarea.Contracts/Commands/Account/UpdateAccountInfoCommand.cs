using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

public record UpdateAccountInfoCommand
{
    public required Guid UserId { get; init; }
    public string? Username { get; init; }
    public string? Email { get; init; }
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