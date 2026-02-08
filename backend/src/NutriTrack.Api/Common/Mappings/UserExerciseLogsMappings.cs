using Mapster;
using NutriTrack.Application.UserExercises.Commands.CreateUserExerciseLog;
using NutriTrack.Application.UserExercises.Commands.UpdateUserExerciseLog;
using NutriTrack.Application.UserExercises.Common;
using NutriTrack.Contracts.UserExerciseLogs;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Contracts.Common;

namespace NutriTrack.Api.Common.Mappings;

public class UserExerciseLogsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateUserExerciseLogRequest, CreateUserExerciseLogCommand>()
            .Map(dest => dest.ExerciseId, src => new ExerciseId(src.ExerciseId));

        config.NewConfig<(Guid Id, UpdateUserExerciseLogRequest Request), UpdateUserExerciseLogCommand>()
            .Map(dest => dest.Id, src => new UserExerciseLogId(src.Id))
            .Map(dest => dest.DurationMinutes, src => src.Request.DurationMinutes)
            .Map(dest => dest.OccurredAtUtc, src => src.Request.OccurredAtLocal.HasValue ? (DateTime?)src.Request.OccurredAtLocal.Value.UtcDateTime : null)
            .Map(dest => dest.OccurredAtLocal, src => src.Request.OccurredAtLocal)
            .Map(dest => dest.LocalDate, src => src.Request.OccurredAtLocal.HasValue ? (DateOnly?)DateOnly.FromDateTime(src.Request.OccurredAtLocal.Value.LocalDateTime) : null)
            .Map(dest => dest.Notes, src => src.Request.Notes);

        config.NewConfig<UserExerciseLogResult, UserExerciseLogResponse>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.ExerciseId, src => src.ExerciseId.Value)
            .Map(dest => dest.ExerciseName, src => src.ExerciseName)
            .Map(dest => dest.Category, src => src.Category.ToString());

        config.NewConfig<PagedResult<UserExerciseLogResult>, PagedResponse<UserExerciseLogResponse>>()
            .Map(dest => dest.Items, src => src.Items)
            .Map(dest => dest.TotalCount, src => src.TotalCount)
            .Map(dest => dest.Page, src => src.Page)
            .Map(dest => dest.PageSize, src => src.PageSize);
    }
}