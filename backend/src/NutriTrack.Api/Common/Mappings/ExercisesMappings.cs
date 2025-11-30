using Mapster;
using NutriTrack.Application.Exercises.Commands.CreateExercise;
using NutriTrack.Application.Exercises.Commands.UpdateExercise;
using NutriTrack.Application.Exercises.Queries.GetExerciseById;
using NutriTrack.Application.Exercises.Queries.ListExercises;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Contracts.Exercises;
using NutriTrack.Domain.Exercises;
using NutriTrack.Application.Common.Models;
using NutriTrack.Contracts.Common;

namespace NutriTrack.Api.Common.Mappings;

public class ExercisesMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateExerciseRequest, CreateExerciseCommand>()
            .Map(dest => dest.Category, src => Enum.Parse<ExerciseCategory>(src.Category, true));

        config.NewConfig<(Guid Id, UpdateExerciseRequest Request), UpdateExerciseCommand>()
            .Map(dest => dest.Id, src => new ExerciseId(src.Id))
            .Map(dest => dest, src => src.Request);

        config.NewConfig<GetExerciseByIdRequest, GetExerciseByIdQuery>()
            .Map(dest => dest.Id, src => new ExerciseId(src.Id));

        config.NewConfig<ListExercisesRequest, ListExercisesQuery>();

        config.NewConfig<ExerciseResult, ExerciseResponse>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.Category, src => src.Category.ToString());

        config.NewConfig<PagedResult<ExerciseResult>, PagedResponse<ExerciseResponse>>()
            .Map(dest => dest.Items, src => src.Items);
    }
}