using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.UserAchievements.Queries.ListUserAchievements;
using NutriTrack.Contracts.Common;
using NutriTrack.Contracts.UserAchievements;

namespace NutriTrack.Api.Controllers;

[Route("api/user-achievements")]
public class UserAchievementsController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public UserAchievementsController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery]int page = 1,
        [FromQuery]int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new ListUserAchievementsQuery(GetUserId(), page, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            achievements => Ok(_mapper.Map<PagedResponse<UserAchievementResponse>>(achievements)),
            errors => Problem(errors)
        );
    }
}