using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.CreateGroupCommand")]
public class CreateGroupCommand
{
    [Id(0)] public required string GroupName { get; set; }
    [Id(1)] public required string Description { get; set; }
    [Id(2)] public required bool IsSystemGroup { get; set; }
}

internal sealed class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        RuleFor(c => c.GroupName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.Description)
            .MaximumLength(500);
    }
}