using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Roles.Common;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ErrorOr<RoleResult>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(
        IRoleRepository roleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<RoleResult>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var name = (request.Name ?? string.Empty).Trim();

        var exists = await _roleRepository.ExistsByNameAsync(name, cancellationToken);
        if (exists)
        {
            return Error.Conflict(code: "Roles.AlreadyExists", description: "Role with the same name already exists.");
        }

        var role = Role.Create(name, request.Description, isSystemRole: false);
        await _roleRepository.AddAsync(role, cancellationToken);

        var permissions = (request.Permissions ?? Array.Empty<string>())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var key in permissions)
        {
            await _rolePermissionRepository.AddAsync(role.Id, key, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var savedPermissions = await _rolePermissionRepository.ListPermissionKeysByRoleIdAsync(role.Id, cancellationToken);
        return role.ToRoleResult(savedPermissions);
    }
}
