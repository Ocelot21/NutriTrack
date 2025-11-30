using ErrorOr;
using MediatR;
using NutriTrack.Application.Meals.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Meals.Commands.CreateMeal;

public record CreateMealCommand(
    UserId UserId,
    string Name,
    DateTime OccurredAtUtc,
    DateTimeOffset OccurredAtLocal,
    DateOnly LocalDate,
    string? Description
) : IRequest<ErrorOr<MealResult>>;
