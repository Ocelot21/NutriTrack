using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Exercises.Queries.ListExercises;

public record ListExercisesQuery(
    UserId? UserId,
    ExerciseListFilters Filters,
    int Page,
    int PageSize) : IRequest<ErrorOr<PagedResult<ExerciseResult>>>;
