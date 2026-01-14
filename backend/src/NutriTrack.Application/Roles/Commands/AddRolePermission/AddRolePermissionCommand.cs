using ErrorOr;
using MediatR;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Roles.Commands.AddRolePermission;

public sealed record AddRolePermissionCommand(RoleId RoleId, string PermissionKey) : IRequest<ErrorOr<Unit>>;
