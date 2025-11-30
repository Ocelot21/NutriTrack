using ErrorOr;
using MediatR;
using NutriTrack.Application.Meals.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Meals.Queries.ListMeals;

public record ListMealsQuery(UserId UserId, DateOnly? From = null, DateOnly? To = null) : IRequest<ErrorOr<IReadOnlyList<MealResult>>>;
