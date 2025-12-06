using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Domain.Authorization;
using NutriTrack.Contracts.Exercises;
using NutriTrack.Application.Exercises.Queries.GetExerciseById;
using NutriTrack.Application.Exercises.Queries.ListExercises;
using NutriTrack.Application.Exercises.Commands.CreateExercise;
using NutriTrack.Application.Exercises.Commands.UpdateExercise;
using NutriTrack.Application.Exercises.Commands.DeleteExercise;
using NutriTrack.Contracts.Common;

namespace NutriTrack.Api.Controllers;

[Route("api/[controller]")]
public sealed class ExercisesController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public ExercisesController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize(Policy = PermissionKeys.Exercises.Read)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetExerciseByIdQuery(new Domain.Exercises.ExerciseId(id));

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            exercise => Ok(_mapper.Map<ExerciseResponse>(exercise)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Exercises.Read)]
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] ListExercisesRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _mapper.Map<ListExercisesQuery>((GetUserId(), request));

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            exercises => Ok(_mapper.Map<PagedResponse<ExerciseResponse>>(exercises)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Exercises.Create)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateExerciseRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateExerciseCommand>(request);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            exercise => CreatedAtAction(
                nameof(GetById),
                new { id = exercise.Id.Value },
                _mapper.Map<ExerciseResponse>(exercise)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Exercises.Update)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateExerciseRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateExerciseCommand>((id, request));

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            exercise => Ok(_mapper.Map<ExerciseResponse>(exercise)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Exercises.Delete)]
    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteExerciseCommand(new NutriTrack.Domain.Exercises.ExerciseId(id));

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}