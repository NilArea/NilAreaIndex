using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.ResetPasswordCommand")]
public record ResetPasswordCommand
{
    [Id(0)] public required string Email { get; init; }
    [Id(1)] public required string ConfirmCode { get; init; }
    [Id(2)] public required string NewPassword { get; init; }
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