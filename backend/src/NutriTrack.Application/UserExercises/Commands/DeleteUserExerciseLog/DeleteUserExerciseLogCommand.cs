using ErrorOr;
using MediatR;
using NutriTrack.Domain.UserExercises;

namespace NutriTrack.Application.UserExercises.Commands.DeleteUserExerciseLog;

public record DeleteUserExerciseLogCommand(UserExerciseLogId Id) : IRequest<ErrorOr<Unit>>;
