using ErrorOr;
using MediatR;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Meals.Commands.UpdateMeal;

public sealed record UpdateMealCommand(
    MealId Id,
    UserId UserId,
    string? Name,
    string? Description,
    DateTimeOffset? OccurredAtLocal
) : IRequest<ErrorOr<Unit>>;