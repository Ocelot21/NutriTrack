using ErrorOr;
using MediatR;
using NutriTrack.Application.Roles.Common;

namespace NutriTrack.Application.Roles.Queries.ListRoles;

public sealed record ListRolesQuery() : IRequest<ErrorOr<IReadOnlyList<RoleResult>>>;
