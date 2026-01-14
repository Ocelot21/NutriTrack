using ErrorOr;
using MediatR;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Users.Commands.SetUserRoleById;

public sealed record SetUserRoleByIdCommand(UserId UserId, RoleId RoleId) : IRequest<ErrorOr<Unit>>;
