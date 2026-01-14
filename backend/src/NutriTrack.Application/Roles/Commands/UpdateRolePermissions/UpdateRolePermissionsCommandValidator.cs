using FluentValidation;

namespace NutriTrack.Application.Roles.Commands.UpdateRolePermissions;

public sealed class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotNull();

        RuleFor(x => x)
            .Must(x => (x.Add?.Count ?? 0) > 0 || (x.Remove?.Count ?? 0) > 0)
            .WithMessage("At least one permission must be added or removed.");

        When(x => x.Add is not null, () =>
        {
            RuleForEach(x => x.Add!)
                .NotEmpty();
        });

        When(x => x.Remove is not null, () =>
        {
            RuleForEach(x => x.Remove!)
                .NotEmpty();
        });
    }
}
