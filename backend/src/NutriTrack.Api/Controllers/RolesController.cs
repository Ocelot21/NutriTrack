using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.Roles.Commands.CreateRole;
using NutriTrack.Application.Roles.Commands.DeleteRole;
using NutriTrack.Application.Roles.Commands.UpdateRolePermissions;
using NutriTrack.Application.Roles.Queries.GetRoleById;
using NutriTrack.Application.Roles.Queries.ListRoles;
using NutriTrack.Contracts.Roles;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Api.Controllers;

[Route("api/admin/roles")]
public sealed class RolesController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public RolesController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize(Policy = PermissionKeys.Roles.Read)]
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        var query = new ListRolesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            roles => Ok(new { Roles = roles.Select(r => _mapper.Map<RoleResponse>(r)) }),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Roles.Read)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRoleByIdQuery(new RoleId(id));
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            role => Ok(_mapper.Map<RoleResponse>(role)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Roles.Create)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateRoleCommand(request.Name, request.Description, request.Permissions);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            role => CreatedAtAction(nameof(GetById), new { id = role.Id.Value }, _mapper.Map<RoleResponse>(role)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.RolePermissions.Manage)]
    [HttpPatch("{id:guid}/permissions")]
    public async Task<IActionResult> UpdatePermissions(
        [FromRoute] Guid id,
        [FromBody] UpdateRolePermissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateRolePermissionsCommand(new RoleId(id), request.Add, request.Remove);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Roles.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteRoleCommand(new RoleId(id));
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}
