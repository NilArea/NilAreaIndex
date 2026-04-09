using FluentValidation;

namespace NilArea.Contracts.Commands.Account;

[GenerateSerializer]
[Alias("NilArea.Contracts.Commands.Account.UpdateGroupCommand")]
public class UpdateGroupCommand
{
    [Id(0)] public required int GroupId { get; set; }
    [Id(1)] public required string GroupName { get; set; }
    [Id(2)] public required string Description { get; set; }
}

internal sealed class UpdateGroupValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupValidator()
    {
        RuleFor(c => c.GroupId)
            .NotEmpty();

        RuleFor(c => c.GroupName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.Description)
            .MaximumLength(500);
    }
}