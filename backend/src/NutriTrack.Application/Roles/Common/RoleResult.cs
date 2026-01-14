using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Roles.Common;

public sealed record RoleResult(
    RoleId Id,
    string Name,
    string? Description,
    bool IsSystemRole,
    bool IsActive,
    IReadOnlyList<string> Permissions);
