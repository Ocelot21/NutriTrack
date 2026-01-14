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
using NutriTrack.Domain.Groceries;
using NutriTrack.Application.Groceries.Queries.ListRecommendedGroceries;
using NutriTrack.Application.Groceries.Queries.GetGroceryByCode;
using NutriTrack.Application.Groceries.Queries.ListGrocerySuggestions;
using NutriTrack.Application.Groceries.Commands.CreateGrocerySuggestion;
using NutriTrack.Application.Groceries.Commands.ApproveGrocery;

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
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetGroceryByIdQuery(new Domain.Groceries.GroceryId(id));

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            grocery => Ok(_mapper.Map<GroceryResponse>(grocery)),
            errors => Problem(errors));
    }    
    
    [Authorize(Policy = PermissionKeys.Groceries.Read)]
    [HttpGet("by-code")]
    public async Task<IActionResult> GetByCode(
        [FromQuery] string code,
        CancellationToken cancellationToken = default)
    {
        var query = new GetGroceryByCodeQuery(code);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            grocery => Ok(_mapper.Map<GroceryResponse>(grocery)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Read)]
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] ListGroceriesRequest request, 
        CancellationToken cancellationToken = default)
    {
        var query = _mapper.Map<ListGroceriesQuery>(((GetUserId()),request));

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            groceries => Ok(_mapper.Map<PagedResponse<GroceryResponse>>(groceries)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Read)]
    [HttpGet("suggestions")]
    public async Task<IActionResult> ListSuggestions(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        var query = new ListGrocerySuggestionsQuery(page, pageSize);
        var result = await _mediator.Send(query);

        return result.Match(
            paged => Ok(_mapper.Map<PagedResponse<GroceryResponse>>(paged)),
            errors => Problem(errors));
    }

    [Authorize]
    [HttpPost("suggestions")]
    public async Task<IActionResult> Suggest(
        [FromForm] CreateGroceryRequest request,
        [FromForm] IFormFile? image,
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateGrocerySuggestionCommand>((request, image));
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            grocery => CreatedAtAction(
                nameof(GetById),
                new { id = grocery.Id.Value },
                _mapper.Map<GroceryResponse>(grocery)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Update)]
    [HttpPost("suggestions/{id:Guid}/approve")]
    public async Task<IActionResult> ApproveSuggestion(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new ApproveGroceryCommand(new GroceryId(id));
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            grocery => Ok(_mapper.Map<GroceryResponse>(grocery)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Create)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] CreateGroceryRequest request,
        [FromForm] IFormFile? image,
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<CreateGroceryCommand>((request, image));

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            grocery => CreatedAtAction(
                nameof(GetById),
                new { id = grocery.Id.Value },
                _mapper.Map<GroceryResponse>(grocery)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Update)]
    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromForm] UpdateGroceryRequest request,
        [FromForm] IFormFile? image,
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<UpdateGroceryCommand>((id, request, image));

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            grocery => Ok(_mapper.Map<GroceryResponse>(grocery)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Delete)]
    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteGroceryCommand(new GroceryId(id));
        var result = await _mediator.Send(command);
        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Groceries.Read)]
    [HttpGet("recommended")]
    public async Task<IActionResult> Recommended(
        [FromQuery] ListRecommendedGroceriesRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new ListRecommendedGroceriesQuery(
            GetUserId(),
            request.Page ?? 1,
            request.PageSize ?? 20);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            groceries => Ok(_mapper.Map<PagedResponse<GroceryResponse>>(groceries)),
            errors => Problem(errors));
    }
}