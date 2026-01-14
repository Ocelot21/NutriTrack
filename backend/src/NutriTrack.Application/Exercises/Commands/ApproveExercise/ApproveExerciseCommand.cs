using ErrorOr;
using MediatR;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Commands.ApproveExercise;

public sealed record ApproveExerciseCommand(ExerciseId Id) : IRequest<ErrorOr<ExerciseResult>>;
