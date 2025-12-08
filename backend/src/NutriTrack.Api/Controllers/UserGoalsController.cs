using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.UserGoals.Commands.CreateUserGoal;
using NutriTrack.Application.UserGoals.Commands.CancelUserGoal;
using NutriTrack.Application.UserGoals.Commands.EvaluateUserProgress;
using NutriTrack.Application.UserGoals.Queries.GetCurrentUserGoal;
using NutriTrack.Application.UserGoals.Queries.ListUserGoals;
using NutriTrack.Contracts.Common;
using NutriTrack.Contracts.UserGoals;

namespace NutriTrack.Api.Controllers;

[Route("api/user-goals")]
public sealed class UserGoalsController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public UserGoalsController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserGoalRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var type = (Domain.Users.NutritionGoal)request.Type;
        var command = new CreateUserGoalCommand(userId, type, request.TargetDate, request.TargetWeightKg);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(_ => NoContent(), errors => Problem(errors));
    }

    [Authorize]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelUserGoalCommand(new NutriTrack.Domain.UserGoals.UserGoalId(id));
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(_ => NoContent(), errors => Problem(errors));
    }

    [Authorize]
    [HttpPost("{id}/evaluate")]
    public async Task<IActionResult> Evaluate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var command = new EvaluateUserProgressCommand(new NutriTrack.Domain.UserGoals.UserGoalId(id), userId);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(changed => Ok(changed), errors => Problem(errors));
    }

    [Authorize]
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var query = new GetCurrentUserGoalQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.Match(goal => Ok(_mapper.Map<UserGoalResponse>(goal)), errors => Problem(errors));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var query = new ListUserGoalsQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.Match(
            paged => Ok(_mapper.Map<PagedResponse<UserGoalResponse>>(paged)),
            errors => Problem(errors));
    }
}
