using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Contracts.Me;
using NutriTrack.Application.Me.Queries.GetMe;
using NutriTrack.Application.Me.Commands.UpdateHealthProfile;
using Microsoft.AspNetCore.Authorization;

namespace NutriTrack.Api.Controllers;

[Route("api/[controller]")]
public class MeController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public MeController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();

        var query = new GetMeQuery(userId);
        var result = await _mediator.Send(query);

        return result.Match(
            user => Ok(_mapper.Map<MeResponse>(user)),
            errors => Problem(errors));
    }

    [Authorize]
    [HttpPut("health-profile")]
    public async Task<IActionResult> UpdateHealthProfile(
        [FromBody] UpdateHealthProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();

        var command = _mapper.Map<UpdateHealthProfileCommand>((userId, request));
        var result = await _mediator.Send(command);

        return result.Match(
            user => Ok(_mapper.Map<MeResponse>(user)),
            errors => Problem(errors));
    }
}