using Mapster;
using NutriTrack.Application.Me.Commands.UpdateHealthProfile;
using NutriTrack.Application.Common.Models;
using NutriTrack.Contracts.Me;
using NutriTrack.Domain.Users;
using NutriTrack.Application.Me.Common;

namespace NutriTrack.Api.Common.Mappings;

public class MeMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(UserId Id, UpdateHealthProfileRequest Request), UpdateHealthProfileCommand>()
            .Map(dest => dest.UserId, src => src.Id)
            .Map(dest => dest.Gender, src =>
                string.IsNullOrWhiteSpace(src.Request.Gender) ?
                (Domain.Users.Gender?)null :
                Enum.Parse<Domain.Users.Gender>(src.Request.Gender, ignoreCase: true))
            .Map(dest => dest.Birthdate, src => src.Request.Birthdate)
            .Map(dest => dest.HeightCm, src => src.Request.HeightCm)
            .Map(dest => dest.WeightKg, src => src.Request.WeightKg)
            .Map(dest => dest.ActivityLevel, src =>
                string.IsNullOrWhiteSpace(src.Request.ActivityLevel) ?
                (Domain.Users.ActivityLevel?)null :
                Enum.Parse<Domain.Users.ActivityLevel>(src.Request.ActivityLevel, ignoreCase: true))
            .Map(dest => dest.NutritionGoal, src =>
                string.IsNullOrWhiteSpace(src.Request.NutritionGoal) ?
                (Domain.Users.NutritionGoal?)null :
                Enum.Parse<Domain.Users.NutritionGoal>(src.Request.NutritionGoal, ignoreCase: true));

        config.NewConfig<UserResult, MeResponse>()
            .Map(dest => dest.Id, src => src.Id.Value);

        config.NewConfig<DailyOverviewResult, DailyOverviewResponse>();
    }
}