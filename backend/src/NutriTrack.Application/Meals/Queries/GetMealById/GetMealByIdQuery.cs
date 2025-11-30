using ErrorOr;
using MediatR;
using NutriTrack.Application.Meals.Common;
using NutriTrack.Domain.Meals;

namespace NutriTrack.Application.Meals.Queries.GetMealById;

public record GetMealByIdQuery(MealId Id) : IRequest<ErrorOr<MealResult>>;
