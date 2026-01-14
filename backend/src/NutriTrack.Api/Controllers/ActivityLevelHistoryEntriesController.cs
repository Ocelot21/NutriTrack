using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.ActivityLevelHistoryEntries.Commands.CreateActivityLevelHistoryEntry;
using NutriTrack.Contracts.ActivityLevelHistoryEntries;
using NutriTrack.Domain.Users;

namespace NutriTrack.Api.Controllers;

[Route("api/activity-level-history-entries")]
public sealed class ActivityLevelHistoryEntriesController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public ActivityLevelHistoryEntriesController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateActivityLevelHistoryEntryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var activityLevel = Enum.Parse<ActivityLevel>(request.ActivityLevel, ignoreCase: true);
        var command = new CreateActivityLevelHistoryEntryCommand(userId, request.EffectiveFrom, activityLevel);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(_ => NoContent(), errors => Problem(errors));
    }
}
