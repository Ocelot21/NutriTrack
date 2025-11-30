using ErrorOr;
using MediatR;
using NutriTrack.Application.UserExercises.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserExercises.Queries.ListUserExerciseLogs;

public record ListUserExerciseLogsQuery(UserId UserId, DateOnly? From = null, DateOnly? To = null) : IRequest<ErrorOr<IReadOnlyList<UserExerciseLogResult>>>;
