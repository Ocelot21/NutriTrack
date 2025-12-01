using ErrorOr;
using MediatR;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Meals;

namespace NutriTrack.Application.Meals.Commands.AddMealItem;

public record AddMealItemCommand(MealId MealId, GroceryId GroceryId, decimal Quantity) : IRequest<ErrorOr<Unit>>;
