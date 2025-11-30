using ErrorOr;
using MediatR;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Meals;

namespace NutriTrack.Application.Meals.Commands.AddMealItem;

public record AddMealItemCommand(MealId MealId, Grocery Grocery, decimal Quantity) : IRequest<ErrorOr<Unit>>;
