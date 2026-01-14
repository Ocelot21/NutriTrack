using ErrorOr;
using MediatR;
using NutriTrack.Application.Roles.Common;

namespace NutriTrack.Application.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    string? Description,
    IReadOnlyList<string>? Permissions) : IRequest<ErrorOr<RoleResult>>;
