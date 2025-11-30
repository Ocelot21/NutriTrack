using ErrorOr;
using MediatR;
using NutriTrack.Domain.Meals;

namespace NutriTrack.Application.Meals.Commands.DeleteMeal;

public record DeleteMealCommand(MealId Id) : IRequest<ErrorOr<Unit>>;
