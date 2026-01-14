using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.TwoFactor.Commands.ConfirmTotp;
using NutriTrack.Application.TwoFactor.Commands.SetupTotp;
using NutriTrack.Application.TwoFactor.Queries.VerifyTotp;
using NutriTrack.Contracts.Totp;

namespace NutriTrack.Api.Controllers;

[Route("api/auth/[controller]")]
[ApiController]
public class TotpController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public TotpController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize]
    [HttpPost("setup")]
    public async Task<IActionResult> Setup(CancellationToken cancellationToken)
    {
        var command = new SetupTotpCommand(GetUserId());
        
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            totp => Ok(_mapper.Map<TotpSetupResponse>(totp)),
            errors => Problem(errors));
    }

    [Authorize]
    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm(
        [FromBody] ConfirmTotpRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmTotpCommand(GetUserId(), request.Code);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }

    [Authorize]
    [HttpPost("verify")]
    public async Task<IActionResult> Verify(
        [FromBody] string code,
        CancellationToken cancellationToken)
    {
        var query = new VerifyTotpQuery(GetUserId(), code);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}