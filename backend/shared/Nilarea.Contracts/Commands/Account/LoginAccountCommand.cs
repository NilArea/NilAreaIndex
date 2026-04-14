using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.LoginAccountCommand")]
public sealed record LoginAccountCommand
{
    [Id(0)] public required string Email { get; init; }
    [Id(1)] public required string Password { get; init; }
}

internal sealed class LoginAccountValidator : AbstractValidator<LoginAccountCommand>
{
    public LoginAccountValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}