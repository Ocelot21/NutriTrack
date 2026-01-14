using FluentValidation;

namespace NutriTrack.Application.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description!)
                .NotEmpty();
        });

        When(x => x.Permissions is not null, () =>
        {
            RuleForEach(x => x.Permissions!)
                .NotEmpty();
        });
    }
}
