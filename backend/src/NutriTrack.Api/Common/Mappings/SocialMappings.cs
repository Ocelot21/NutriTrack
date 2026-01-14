using Mapster;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Social.Common;
using NutriTrack.Contracts.Common;
using NutriTrack.Contracts.Social;
using NutriTrack.Contracts.UserAchievements;

namespace NutriTrack.Api.Common.Mappings;

public static class SocialMappings
{
    public static void Configure(TypeAdapterConfig config)
    {
        //config.NewConfig<SocialPostResult, SocialPostResponse>()
        //    .Map(dest => dest.Id, src => src.Id.Value)
        //    .Map(dest => dest.Type, src => (int)src.Type)
        //    .Map(dest => dest.Visibility, src => (int)src.Visibility)
        //    .Map(dest => dest.LocalTime, src => src.LocalTime)
        //    .Map(dest => dest.Text, src => src.Text)
        //    .Map(dest => dest.Author, src => new SocialPostAuthorResponse(
        //        src.Author.Id.Value,
        //        src.Author.Username.Value,
        //        src.Author.AvatarUrl
        //    ))
        //    .Map(dest => dest.UserAchievement, src => src.UserAchievement)
        //    .Map(dest => dest.Meal, src => src.MealSnapshot);

        //config.NewConfig<PagedResult<SocialPostResult>, PagedResponse<SocialPostResponse>>()
        //    .Map(dest => dest.Items, src => src.Items.Select(i => 
        //        new SocialPostResponse(
        //            i.Id.Value,
        //            new SocialPostAuthorResponse(
        //                i.Author.Id.Value,
        //                i.Author.Username.Value,
        //                i.Author.AvatarUrl),
        //            (int)i.Type,
        //            (int)i.Visibility,
        //            i.LocalTime,
        //            i.Text,
        //            i.UserAchievement == null ?
        //            null :
        //            new UserAchievementResponse(
        //                i.UserAchievement!.Id.Value,
        //                i.UserAchievement!.UserId.Value,
        //                i.UserAchievement!.AchievementId.Value,
        //                i.UserAchievement!.Achievement.Key,
        //                i.UserAchievement!.Achievement.Title,
        //                i.UserAchievement!.Achievement.Description,
        //                i.UserAchievement!.Achievement.Points,
        //                i.UserAchievement!.Achievement.Category.ToString(),
        //                i.UserAchievement!.Achievement.IconName,
        //                i.UserAchievement.LocalDateEarned),
        //            i.MealSnapshot == null ?
        //            null :
        //            new MealSnapshotResponse(
        //                i.MealSnapshot.Id.Value,
        //                i.MealSnapshot.MealId.HasValue ? i.MealSnapshot.MealId.Value.Value : null,
        //                i.MealSnapshot.MealName,
        //                i.MealSnapshot.MealDate,
        //                i.MealSnapshot.TotalCalories,
        //                i.MealSnapshot.TotalProtein,
        //                i.MealSnapshot.TotalCarbohydrates,
        //                i.MealSnapshot.TotalFats,
        //                i.MealSnapshot.ItemsJson))));
    }
}