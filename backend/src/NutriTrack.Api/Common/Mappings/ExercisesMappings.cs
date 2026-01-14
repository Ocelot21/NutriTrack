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
using NutriTrack.Domain.Users;
using Microsoft.AspNetCore.Http;
using NutriTrack.Application.Exercises.Commands.CreateExerciseSuggestion;

namespace NutriTrack.Api.Common.Mappings;

public class ExercisesMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(CreateExerciseRequest Request, IFormFile? Image), CreateExerciseCommand>()
            .ConstructUsing(src => new CreateExerciseCommand(
                src.Request.Name,
                Enum.Parse<ExerciseCategory>(src.Request.Category, true),
                src.Request.DefaultCaloriesPerMinute,
                src.Request.Description,
                src.Image != null ? src.Image.OpenReadStream() : null,
                src.Image != null ? src.Image.FileName : null,
                src.Image != null ? src.Image.ContentType : null));

        config.NewConfig<(Guid Id, UpdateExerciseRequest Request, IFormFile? Image), UpdateExerciseCommand>()
            .ConstructUsing(src => new UpdateExerciseCommand(
                new ExerciseId(src.Id),
                src.Request.Name,
                string.IsNullOrWhiteSpace(src.Request.Category) ? null : Enum.Parse<ExerciseCategory>(src.Request.Category!, true),
                src.Request.DefaultCaloriesPerMinute,
                src.Request.Description,
                src.Image != null ? src.Image.OpenReadStream() : null,
                src.Image != null ? src.Image.FileName : null,
                src.Image != null ? src.Image.ContentType : null,
                src.Request.IsApproved,
                src.Request.IsDeleted));

        config.NewConfig<GetExerciseByIdRequest, GetExerciseByIdQuery>()
            .Map(dest => dest.Id, src => new ExerciseId(src.Id));

        config.NewConfig<ListExercisesRequest, ListExercisesQuery>();

        config.NewConfig<ExerciseResult, ExerciseResponse>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.Category, src => src.Category.ToString());

        config.NewConfig<PagedResult<ExerciseResult>, PagedResponse<ExerciseResponse>>()
            .Map(dest => dest.Items, src => src.Items);

        config.NewConfig<(UserId UserId, ListExercisesRequest Request), ListExercisesQuery>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.Request)
            .Map(dest => dest.Filters, src => src.Request);

        config.NewConfig<(CreateExerciseRequest Request, IFormFile? Image), CreateExerciseSuggestionCommand>()
            .ConstructUsing(src => new CreateExerciseSuggestionCommand(
                src.Request.Name,
                Enum.Parse<ExerciseCategory>(src.Request.Category, true),
                src.Request.DefaultCaloriesPerMinute,
                src.Request.Description,
                src.Image != null ? src.Image.OpenReadStream() : null,
                src.Image != null ? src.Image.FileName : null,
                src.Image != null ? src.Image.ContentType : null));
    }
}