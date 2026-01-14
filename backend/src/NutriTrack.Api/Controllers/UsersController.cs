using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.Users.Commands.DeleteUserById;
using NutriTrack.Application.Users.Commands.SetUserRoleById;
using NutriTrack.Application.Users.Queries.GetUserById;
using NutriTrack.Application.Users.Queries.ListUsers;
using NutriTrack.Contracts.Common;
using NutriTrack.Contracts.Users;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Users;

namespace NutriTrack.Api.Controllers;

[Route("api/admin/[controller]")]
public sealed class UsersController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public UsersController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize(Policy = PermissionKeys.Users.Read)]
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new ListUsersQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            users => Ok(_mapper.Map<PagedResponse<UserResponse>>(users)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Users.Read)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(new UserId(id));
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            user => Ok(_mapper.Map<UserResponse>(user)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Users.Manage)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteUserByIdCommand(new UserId(id));
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Users.SetRole)]
    [HttpPut("{id:guid}/role")]
    public async Task<IActionResult> SetRole(
        [FromRoute] Guid id,
        [FromBody] SetUserRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new SetUserRoleByIdCommand(
            new UserId(id),
            new RoleId(request.RoleId));

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}
