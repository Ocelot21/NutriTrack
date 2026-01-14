namespace NutriTrack.Contracts.Roles;

public sealed record CreateRoleRequest(
    string Name,
    string? Description,
    IReadOnlyList<string>? Permissions);
