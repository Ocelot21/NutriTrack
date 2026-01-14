using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Contracts.Me;
using NutriTrack.Application.Me.Queries.GetMe;
using NutriTrack.Application.Me.Commands.UpdateHealthProfile;
using Microsoft.AspNetCore.Authorization;
using NutriTrack.Application.Me.Queries.GetDailyOverview;
using NutriTrack.Application.Me.Commands.ChangePassword;
using NutriTrack.Application.Me.Commands.UpdateUserProfile;
using NutriTrack.Application.Me.Commands.UploadAvatar;
using Microsoft.AspNetCore.Http;

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
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            user => Ok(_mapper.Map<MeResponse>(user)),
            errors => Problem(errors));
    }

    [Authorize]
    [HttpGet("daily-overview/{localDate:datetime}")]
    public async Task<IActionResult> GetDailyOverview(
        DateOnly localDate,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var query = new GetDailyOverviewQuery(userId, localDate);
        var result = await _mediator.Send(query, cancellationToken);
        return result.Match(
            overview => Ok(_mapper.Map<DailyOverviewResponse>(overview)),
            errors => Problem(errors));
    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var command = _mapper.Map<UpdateUserProfileCommand>((userId, request));
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }

    [Authorize]
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(
        [FromForm] IFormFile avatar,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var command = _mapper.Map<UploadAvatarCommand>((userId, avatar));
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
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
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            user => Ok(_mapper.Map<MeResponse>(user)),
            errors => Problem(errors));
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var command = _mapper.Map<ChangePasswordCommand>((userId, request));
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}