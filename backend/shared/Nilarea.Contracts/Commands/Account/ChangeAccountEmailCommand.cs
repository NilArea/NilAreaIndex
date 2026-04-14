using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.ChangeAccountEmailCommand")]
public sealed record ChangeAccountEmailCommand
{
    [Id(0)] public required long UserId { get; init; }
    [Id(1)] public required string NewEmail { get; init; }
}

internal sealed class UpdateAccountInfoValidator : AbstractValidator<ChangeAccountEmailCommand>
{
    public UpdateAccountInfoValidator()
    {
        RuleFor(r => r.UserId)
            .NotEmpty().WithMessage("User ID is required.");
        RuleFor(r => r.NewEmail)
            .NotEmpty().WithMessage("NewEmail is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}