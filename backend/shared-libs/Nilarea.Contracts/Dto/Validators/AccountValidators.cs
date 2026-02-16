using FluentValidation;

namespace NilArea.Contracts.Dto.Validators;

internal sealed class RegisterRequestValidator : AbstractValidator<AccountRegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(4).WithMessage("Username must be at least 4 characters long.");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}

internal sealed class LoginRequestValidator : AbstractValidator<AccountLoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}
