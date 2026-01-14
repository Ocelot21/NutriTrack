using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Roles.Common;

namespace NutriTrack.Application.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, ErrorOr<RoleResult>>
{
    private readonly IRoleRepository _repo;
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public GetRoleByIdQueryHandler(
        IRoleRepository repo,
        IRolePermissionRepository rolePermissionRepository)
    {
        _repo = repo;
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<ErrorOr<RoleResult>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _repo.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
        {
            return Errors.Roles.NotFound;
        }

        var permissions = await _rolePermissionRepository.ListPermissionKeysByRoleIdAsync(role.Id, cancellationToken);
        return role.ToRoleResult(permissions);
    }
}
