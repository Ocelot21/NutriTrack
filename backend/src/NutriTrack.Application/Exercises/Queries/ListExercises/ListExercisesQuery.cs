using ErrorOr;
using MediatR;
using NutriTrack.Application.Exercises.Common;

namespace NutriTrack.Application.Exercises.Queries.ListExercises;

public record ListExercisesQuery(int? Page = null, int? PageSize = null, bool ApprovedOnly = true) : IRequest<ErrorOr<IReadOnlyList<ExerciseResult>>>;
