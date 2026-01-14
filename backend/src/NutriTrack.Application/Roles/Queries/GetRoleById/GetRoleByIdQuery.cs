using ErrorOr;
using MediatR;
using NutriTrack.Application.Roles.Common;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdQuery(RoleId RoleId) : IRequest<ErrorOr<RoleResult>>;
