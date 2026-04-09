using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

public record ResetPasswordCommand
{
    public required string Email { get; init; }
    public required string ConfirmCode { get; init; }
    public required string NewPassword { get; init; }
}

internal sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.ConfirmCode)
            .NotEmpty().WithMessage("Confirm code is required.");
        RuleFor(r => r.NewPassword)
            .NotEmpty().WithMessage("New password is required.");
    }
}