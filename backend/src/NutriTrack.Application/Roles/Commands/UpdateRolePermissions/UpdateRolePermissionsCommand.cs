using ErrorOr;
using MediatR;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Roles.Commands.UpdateRolePermissions;

public sealed record UpdateRolePermissionsCommand(
    RoleId RoleId,
    IReadOnlyList<string>? Add,
    IReadOnlyList<string>? Remove) : IRequest<ErrorOr<Unit>>;
