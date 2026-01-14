namespace NutriTrack.Contracts.Roles;

public sealed record UpdateRolePermissionsRequest(
    IReadOnlyList<string>? Add,
    IReadOnlyList<string>? Remove);
