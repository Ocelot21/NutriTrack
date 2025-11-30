using ErrorOr;
using MediatR;
using NutriTrack.Domain.Meals;

namespace NutriTrack.Application.Meals.Commands.UpdateMealItem;

public record UpdateMealItemCommand(MealId MealId, MealItemId MealItemId, decimal Quantity) : IRequest<ErrorOr<Unit>>;
