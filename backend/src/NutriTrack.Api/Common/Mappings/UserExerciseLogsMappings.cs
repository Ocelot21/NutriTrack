using Mapster;
using NutriTrack.Application.UserExercises.Commands.CreateUserExerciseLog;
using NutriTrack.Application.UserExercises.Commands.DeleteUserExerciseLog;
using NutriTrack.Application.UserExercises.Common;
using NutriTrack.Contracts.UserExerciseLogs;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Api.Common.Mappings;

public class UserExerciseLogsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateUserExerciseLogRequest, CreateUserExerciseLogCommand>()
            .Map(dest => dest.ExerciseId, src => new ExerciseId(src.ExerciseId));

        config.NewConfig<DeleteUserExerciseLogRequest, DeleteUserExerciseLogCommand>()
            .Map(dest => dest.Id, src => new UserExerciseLogId(src.Id));

        config.NewConfig<UserExerciseLogResult, UserExerciseLogResponse>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.ExerciseId, src => src.ExerciseId.Value)
            .Map(dest => dest.ExerciseName, src => src.ExerciseName)
            .Map(dest => dest.Category, src => src.Category.ToString());

        config.NewConfig<PagedResult<UserExerciseLogResult>, NutriTrack.Contracts.Common.PagedResponse<UserExerciseLogResponse>>()
            .Map(dest => dest.Items, src => src.Items)
            .Map(dest => dest.TotalCount, src => src.TotalCount)
            .Map(dest => dest.Page, src => src.Page)
            .Map(dest => dest.PageSize, src => src.PageSize);
    }
}