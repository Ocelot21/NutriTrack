using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.Authentication.Commands.Login;
using NutriTrack.Application.Authentication.Commands.Register;
using NutriTrack.Contracts.Authentication;

namespace NutriTrack.Api.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public AuthController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<RegisterCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            result => Ok(_mapper.Map<AuthenticationResponse>(result)),
            errors => Problem(errors)
            );
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<LoginCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            result => Ok(_mapper.Map<AuthenticationResponse>(result)),
            errors => Problem(errors)
            );
    }
}