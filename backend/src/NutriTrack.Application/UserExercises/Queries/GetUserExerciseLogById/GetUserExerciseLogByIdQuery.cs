using ErrorOr;
using MediatR;
using NutriTrack.Application.UserExercises.Common;
using NutriTrack.Domain.UserExercises;

namespace NutriTrack.Application.UserExercises.Queries.GetUserExerciseLogById;

public record GetUserExerciseLogByIdQuery(UserExerciseLogId Id) : IRequest<ErrorOr<UserExerciseLogResult>>;
