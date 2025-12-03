using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Domain.Authorization;
using NutriTrack.Contracts.UserExerciseLogs;
using NutriTrack.Application.UserExercises.Queries.GetUserExerciseLogById;
using NutriTrack.Application.UserExercises.Queries.ListUserExerciseLogs;
using NutriTrack.Application.UserExercises.Commands.CreateUserExerciseLog;
using NutriTrack.Application.UserExercises.Commands.DeleteUserExerciseLog;
using NutriTrack.Domain.UserExercises;

namespace NutriTrack.Api.Controllers;

[Route("api/user-exercise-logs")]
public sealed class UserExerciseLogsController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public UserExerciseLogsController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize(Policy = PermissionKeys.UserExerciseLogs.Read_Own)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserExerciseLogByIdQuery(new UserExerciseLogId(id));

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            log => Ok(_mapper.Map<UserExerciseLogResponse>(log)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.UserExerciseLogs.Read_Own)]
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var query = new ListUserExerciseLogsQuery(userId, from, to);

        var result = await _mediator.Send(query, cancellationToken);
        return result.Match(
            logs => Ok(logs.Select(l => _mapper.Map<UserExerciseLogResponse>(l))),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.UserExerciseLogs.Create)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserExerciseLogRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var command = _mapper.Map<CreateUserExerciseLogCommand>(request) with { UserId = userId };

        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            log => CreatedAtAction(nameof(GetById), new { id = log.Id.Value }, _mapper.Map<UserExerciseLogResponse>(log)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.UserExerciseLogs.Read_Own)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteUserExerciseLogCommand(new UserExerciseLogId(id));

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}