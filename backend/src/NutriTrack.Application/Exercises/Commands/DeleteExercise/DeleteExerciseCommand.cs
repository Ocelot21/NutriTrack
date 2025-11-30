using ErrorOr;
using MediatR;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Commands.DeleteExercise;

public record DeleteExerciseCommand(ExerciseId Id) : IRequest<ErrorOr<Unit>>;
