using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Exercises.Common;

namespace NutriTrack.Application.Exercises.Queries.ListExerciseSuggestions;

public sealed record ListExerciseSuggestionsQuery(int Page = 1, int PageSize = 20)
    : IRequest<ErrorOr<PagedResult<ExerciseResult>>>;
