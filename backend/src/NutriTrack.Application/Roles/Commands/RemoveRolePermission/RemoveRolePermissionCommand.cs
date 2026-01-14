using ErrorOr;
using MediatR;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Roles.Commands.RemoveRolePermission;

public sealed record RemoveRolePermissionCommand(RoleId RoleId, string PermissionKey) : IRequest<ErrorOr<Unit>>;
