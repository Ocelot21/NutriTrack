using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.Metadata.Queries.GetSystemTimeZones;
using NutriTrack.Application.Metadata.Queries.ListCountries;
using NutriTrack.Application.Metadata.Queries.ListPermissionKeys;
using NutriTrack.Contracts.Metadata;

namespace NutriTrack.Api.Controllers;

[Route("api/[controller]")]
public class MetadataController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public MetadataController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("time-zones")]
    public async Task<IActionResult> GetTimeZones()
    {
        var result = await _mediator.Send(new GetSystemTimeZonesQuery());
        return result.Match(
            timezones => Ok(new { TimeZones = timezones }),
            errors => Problem(errors)
        );
    }

    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries()
    {
        var result = await _mediator.Send(new ListCountriesQuery());
        return result.Match(
            countries => Ok(
                _mapper.Map<CountryListResponse>(countries)),
            errors => Problem(errors)
        );
    }

    [HttpGet("permission-keys")]
    public async Task<IActionResult> GetPermissionKeys(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListPermissionKeysQuery(), cancellationToken);
        return result.Match(
            keys => Ok(new { PermissionKeys = keys }),
            errors => Problem(errors));
    }
}