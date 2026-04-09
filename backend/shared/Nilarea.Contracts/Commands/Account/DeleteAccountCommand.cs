using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.DeleteAccountCommand")]
public class DeleteAccountCommand
{
    [Id(0)] public required string Email { get; set; }
    [Id(2)] public required string Password { get; set; }
    [Id(1)] public required string ConfirmKey { get; set; }
}

internal sealed class DeleteAccountValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
        RuleFor(r => r.ConfirmKey)
            .NotEmpty().WithMessage("Confirm key is required.");
    }
}