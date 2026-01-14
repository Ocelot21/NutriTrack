using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Roles.Common;

public static class RoleMappings
{
    public static RoleResult ToRoleResult(this Role role, IReadOnlyList<string>? permissions = null)
        => new(role.Id, role.Name, role.Description, role.IsSystemRole, role.IsActive, permissions ?? Array.Empty<string>());
}
