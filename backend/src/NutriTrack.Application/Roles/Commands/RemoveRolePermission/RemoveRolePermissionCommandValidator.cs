using FluentValidation;

namespace NutriTrack.Application.Roles.Commands.RemoveRolePermission;

public sealed class RemoveRolePermissionCommandValidator : AbstractValidator<RemoveRolePermissionCommand>
{
    public RemoveRolePermissionCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotNull();

        RuleFor(x => x.PermissionKey)
            .NotEmpty();
    }
}
