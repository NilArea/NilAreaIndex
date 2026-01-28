using FluentValidation;
using NilArea.Contracts.Dto;

namespace NilArea.Contracts.Validators;

public static class AccountValidators
{
    internal class RegisterRequestValidator : AbstractValidator<Requests.RegisterAccount>
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

    internal class LoginRequestValidator : AbstractValidator<Requests.LoginAccount>
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
}
