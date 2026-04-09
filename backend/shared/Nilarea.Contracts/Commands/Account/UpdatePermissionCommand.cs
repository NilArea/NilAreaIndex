using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.UpdatePermissionCommand")]
public class UpdatePermissionCommand
{
    [Id(0)] public required short PermissionId { get; set; }
    [Id(1)] public required string PermissionName { get; set; }
    [Id(2)] public required string Description { get; set; }
}

internal sealed class UpdatePermissionValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionValidator()
    {
        RuleFor(c => c.PermissionId)
            .NotEmpty();

        RuleFor(c => c.PermissionName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.Description)
            .MaximumLength(500);
    }
}