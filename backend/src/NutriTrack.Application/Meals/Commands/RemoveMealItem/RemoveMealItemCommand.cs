using ErrorOr;
using MediatR;
using NutriTrack.Domain.Meals;

namespace NutriTrack.Application.Meals.Commands.RemoveMealItem;

public record RemoveMealItemCommand(MealId MealId, MealItemId MealItemId) : IRequest<ErrorOr<Unit>>;
