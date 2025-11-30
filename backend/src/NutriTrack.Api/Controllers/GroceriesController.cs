using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Contracts.Groceries;
using NutriTrack.Application.Groceries.Commands.CreateGrocery;
using NutriTrack.Application.Groceries.Commands.UpdateGrocery;
using NutriTrack.Application.Groceries.Queries.GetGroceryById;
using NutriTrack.Application.Groceries.Queries.ListGroceries;
using Microsoft.AspNetCore.Authorization;
using NutriTrack.Domain.Authorization;
using NutriTrack.Application.Groceries.Commands.DeleteGrocery;
using NutriTrack.Contracts.Common;

namespace NutriTrack.Api.Controllers;

[Route("api/[controller]")]
public sealed class GroceriesController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public GroceriesController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize(Policy = PermissionKeys.Groceries.Read)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var request = new GetGroceryByIdRequest(id);
        var query = _mapper.Map<GetGroceryByIdQuery>(request);
        var result = await _mediator.Send(query);
        return result.Match(
            grocery => Ok(_mapper.Map<GroceryResponse>(grocery)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Read)]
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListGroceriesRequest request)
    {
        var query = _mapper.Map<ListGroceriesQuery>(request);
        var result = await _mediator.Send(query);
        return result.Match(
            groceries => Ok(_mapper.Map<PagedResponse<GroceryResponse>>(groceries)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroceryRequest request)
    {
        var command = _mapper.Map<CreateGroceryCommand>(request);
        var result = await _mediator.Send(command);
        return result.Match(
            grocery => CreatedAtAction(
                nameof(GetById),
                new { id = grocery.Id.Value },
                _mapper.Map<GroceryResponse>(grocery)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Update)]
    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateGroceryRequest request)
    {
        var command = _mapper.Map<UpdateGroceryCommand>((id, request));
        var result = await _mediator.Send(command);
        return result.Match(
            grocery => Ok(_mapper.Map<GroceryResponse>(grocery)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Delete)]
    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteGroceryCommand(new Domain.Groceries.GroceryId(id));
        var result = await _mediator.Send(command);
        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}