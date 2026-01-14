namespace NutriTrack.Contracts.Roles;

public sealed record RoleResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystemRole,
    bool IsActive,
    IReadOnlyList<string> Permissions);
