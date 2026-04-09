using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.CreatePermissionCommand")]
public class CreatePermissionCommand
{
    [Id(0)] public required string PermissionName { get; set; }
    [Id(1)] public required string Description { get; set; }
}

internal sealed class CreatePermissionValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionValidator()
    {
        RuleFor(c => c.PermissionName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.Description)
            .MaximumLength(500);
    }
}