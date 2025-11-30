using ErrorOr;
using MediatR;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Queries.GetExerciseById;

public record GetExerciseByIdQuery(ExerciseId Id) : IRequest<ErrorOr<ExerciseResult>>;
