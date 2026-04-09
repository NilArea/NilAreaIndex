using FluentValidation;

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

internal sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
        RuleFor(r => r.NewPassword)
            .NotEmpty().WithMessage("New password is required.");
        RuleFor(r => r.ConfirmKey)
            .NotEmpty().WithMessage("Confirm key is required.");
    }
}