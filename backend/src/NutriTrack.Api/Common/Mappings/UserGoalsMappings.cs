using Mapster;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.UserGoals.Common;
using NutriTrack.Contracts.Common;
using NutriTrack.Contracts.UserGoals;

namespace NutriTrack.Api.Common.Mappings;

public sealed class UserGoalsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserGoalResult, UserGoalResponse>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.UserId, src => src.UserId.Value)
                .Map(dest => dest.Type, src => (int)src.Type)
                .Map(dest => dest.Status, src => (int)src.Status)
                .Map(dest => dest.StartDate, src => src.StartDate)
                .Map(dest => dest.TargetDate, src => src.TargetDate)
                .Map(dest => dest.StartWeightKg, src => src.StartWeightKg)
                .Map(dest => dest.TargetWeightKg, src => src.TargetWeightKg)
                .Map(dest => dest.CompletedAtUtc, src => src.CompletedAtUtc)
                .Map(dest => dest.FailedAtUtc, src => src.FailedAtUtc);

        config.NewConfig<PagedResult<UserGoalResult>, PagedResponse<UserGoalResponse>>()
            .Map(dest => dest.Items, src => src.Items);
    }
}
