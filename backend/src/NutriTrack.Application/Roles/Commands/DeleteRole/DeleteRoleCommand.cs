using ErrorOr;
using MediatR;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Roles.Commands.DeleteRole;

public sealed record DeleteRoleCommand(RoleId RoleId) : IRequest<ErrorOr<Unit>>;
