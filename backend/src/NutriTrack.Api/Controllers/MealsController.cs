using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Domain.Authorization;
using NutriTrack.Contracts.Meals;
using NutriTrack.Application.Meals.Queries.GetMealById;
using NutriTrack.Application.Meals.Queries.ListMeals;
using NutriTrack.Application.Meals.Commands.CreateMeal;
using NutriTrack.Application.Meals.Commands.DeleteMeal;
using NutriTrack.Application.Meals.Commands.AddMealItem;
using NutriTrack.Application.Meals.Commands.UpdateMealItem;
using NutriTrack.Application.Meals.Commands.RemoveMealItem;
using NutriTrack.Domain.Meals;

namespace NutriTrack.Api.Controllers;

[Route("api/[controller]")]
public sealed class MealsController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public MealsController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize(Policy = PermissionKeys.Meals.Read_All)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMealByIdQuery(new MealId(id));
        var result = await _mediator.Send(query, cancellationToken);
        return result.Match(
            meal => Ok(_mapper.Map<MealResponse>(meal)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Meals.Read_Own)]
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var query = new ListMealsQuery(userId, from, to);
        var result = await _mediator.Send(query, cancellationToken);
        return result.Match(
            meals => Ok(meals.Select(m => _mapper.Map<MealResponse>(m))),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Meals.Create)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateMealRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var command = _mapper.Map<CreateMealCommand>(request) with { UserId = userId };

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            meal => CreatedAtAction(nameof(GetById), new { id = meal.Id.Value }, _mapper.Map<MealResponse>(meal)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Meals.Create)]
    [HttpPost("{mealId}/items")]
    public async Task<IActionResult> AddItem(
        [FromRoute] Guid mealId,
        [FromBody] AddMealItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<AddMealItemCommand>(request) with { MealId = new MealId(mealId) };

        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Meals.Update_Own)]
    [HttpPut("{mealId}/items/{itemId}")]
    public async Task<IActionResult> UpdateItem(
        [FromRoute] Guid mealId,
        [FromRoute] Guid itemId,
        [FromBody] decimal quantity,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateMealItemCommand(
            new MealId(mealId),
            new MealItemId(itemId),
            quantity);

        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Meals.Update_Own)]
    [HttpDelete("{mealId}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(
        [FromRoute] Guid mealId,
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveMealItemCommand(new MealId(mealId), new MealItemId(itemId));
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Meals.Read_Own)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteMealCommand(new MealId(id));
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }
}