using Mapster;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.UserAchievements.Common;
using NutriTrack.Contracts.Common;
using NutriTrack.Contracts.UserAchievements;

namespace NutriTrack.Api.Common.Mappings;

public class UserAchievementsMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserAchievementResult, UserAchievementResponse>()
        .Map(dest => dest.Id, src => src.Id.Value)
        .Map(dest => dest.UserId, src => src.UserId.Value)
        .Map(dest => dest.AchievementId, src => src.AchievementId.Value)
        .Map(dest => dest.Key, src => src.Key)
        .Map(dest => dest.Title, src => src.Title)
        .Map(dest => dest.Description, src => src.Description)
        .Map(dest => dest.Points, src => src.Points)
        .Map(dest => dest.Category, src => src.Category.ToString())
        .Map(dest => dest.IconName, src => src.IconName)
        .Map(dest => dest.LocalDateEarned, src => src.LocalDateEarned);

        config.NewConfig<PagedResult<UserAchievementResult>, PagedResponse<UserAchievementResponse>>()
        .Map(dest => dest.Items, src => src.Items);
    }
}