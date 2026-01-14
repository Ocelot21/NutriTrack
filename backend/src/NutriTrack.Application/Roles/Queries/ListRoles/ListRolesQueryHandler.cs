using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Roles.Common;

namespace NutriTrack.Application.Roles.Queries.ListRoles;

public sealed class ListRolesQueryHandler : IRequestHandler<ListRolesQuery, ErrorOr<IReadOnlyList<RoleResult>>>
{
    private readonly IRoleRepository _repo;

    public ListRolesQueryHandler(IRoleRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<IReadOnlyList<RoleResult>>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _repo.ListAsync(cancellationToken);
        return roles.Select(r => r.ToRoleResult()).ToList();
    }
}
