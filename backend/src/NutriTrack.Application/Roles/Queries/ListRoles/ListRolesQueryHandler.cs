using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Roles.Common;

namespace NutriTrack.Application.Roles.Queries.ListRoles;

public sealed class ListRolesQueryHandler : IRequestHandler<ListRolesQuery, ErrorOr<IReadOnlyList<RoleResult>>>
{
    private readonly IRoleRepository _repo;
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public ListRolesQueryHandler(IRoleRepository repo, IRolePermissionRepository rolePermissionRepository)
    {
        _repo = repo;
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<RoleResult>>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _repo.ListAsync(cancellationToken);

        var results = new List<RoleResult>();
        foreach (var role in roles)
        {
            var permissions = await _rolePermissionRepository.ListPermissionKeysByRoleIdAsync(role.Id, cancellationToken);
            results.Add(role.ToRoleResult(permissions));
        }

        return results;
    }
}
