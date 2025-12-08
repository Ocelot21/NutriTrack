using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.WeightHistoryEntries.Commands.CreateWeightHistoryEntry;
using NutriTrack.Application.WeightHistoryEntries.Queries.ListWeightEntriesInRange;
using NutriTrack.Contracts.WeightHistoryEntries;

namespace NutriTrack.Api.Controllers;

[Route("api/weight-history-entries")]
public sealed class WeightHistoryEntriesController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public WeightHistoryEntriesController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWeightHistoryEntryRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var command = new CreateWeightHistoryEntryCommand(userId, request.Date, request.WeightKg);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(_ => NoContent(), errors => Problem(errors));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var query = new ListWeightHistoryEntriesInRangeQuery(userId, from, to);
        var result = await _mediator.Send(query, cancellationToken);
        return result.Match(
            entries => Ok(new { Entries = _mapper.Map<IReadOnlyList<WeightHistoryEntryResponse>>(entries) }),
            errors => Problem(errors));
    }
}