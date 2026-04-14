using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.RegisterAccountCommand")]
public sealed record RegisterAccountCommand
{
    [Id(0)] public required string Email { get; init; }
    [Id(1)] public required string Password { get; init; }
    [Id(2)] public required string Username { get; init; }
    [Id(3)] public required string ConfirmKey { get; init; }
}

internal sealed class RegisterAccountValidator : AbstractValidator<RegisterAccountCommand>
{
    public RegisterAccountValidator()
    {
        RuleFor(r => r.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(4).WithMessage("Username must be at least 4 characters long.");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        RuleFor(r => r.ConfirmKey)
            .NotEmpty().WithMessage("Confirm key is required.");
    }
}