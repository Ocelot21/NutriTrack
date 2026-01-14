using FluentValidation;

namespace NutriTrack.Application.Roles.Commands.AddRolePermission;

public sealed class AddRolePermissionCommandValidator : AbstractValidator<AddRolePermissionCommand>
{
    public AddRolePermissionCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotNull();

        RuleFor(x => x.PermissionKey)
            .NotEmpty();
    }
}
