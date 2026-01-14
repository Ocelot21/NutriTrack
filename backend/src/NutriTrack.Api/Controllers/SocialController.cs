using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Social.Commands.CreateTextPost;
using NutriTrack.Application.Social.Commands.DeleteSocialPost;
using NutriTrack.Application.Social.Commands.ShareAchievementPost;
using NutriTrack.Application.Social.Commands.ShareDailyOverviewSnapshot;
using NutriTrack.Application.Social.Commands.ShareGoalProgressSnapshot;
using NutriTrack.Application.Social.Common;
using NutriTrack.Application.Social.Queries;
using NutriTrack.Application.Social.Queries.GetSocialProfile;
using NutriTrack.Contracts.Common;
using NutriTrack.Contracts.Social;
using NutriTrack.Contracts.UserAchievements;

namespace NutriTrack.Api.Controllers;

[Route("api/me/social")]
[ApiController]
public sealed class SocialController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SocialController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet("profile/{userId}")]
    public async Task<IActionResult> GetProfile(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = new GetSocialProfileQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            profile => Ok(new SocialProfileResponse(
                profile.UserId,
                profile.Username,
                profile.AvatarUrl,
                profile.TotalPoints,
                profile.Posts.Select(p => new SocialPostResponse(
                    p.Id.Value,
                    new SocialPostAuthorResponse(p.Author.Id.Value, p.Author.Username.Value, p.Author.AvatarUrl),
                    (int)p.Type,
                    (int)p.Visibility,
                    p.LocalTime,
                    p.Text,
                    p.UserAchievement == null
                        ? null
                        : new UserAchievementResponse(
                            p.UserAchievement!.Id.Value,
                            p.UserAchievement!.UserId.Value,
                            p.UserAchievement!.AchievementId.Value,
                            p.UserAchievement!.Achievement.Key,
                            p.UserAchievement!.Achievement.Title,
                            p.UserAchievement!.Achievement.Description,
                            p.UserAchievement!.Achievement.Points,
                            p.UserAchievement!.Achievement.Category.ToString(),
                            p.UserAchievement!.Achievement.IconName,
                            p.UserAchievement.LocalDateEarned),
                    p.DailyOverviewSnapshot == null
                        ? null
                        : new DailyOverviewSnapshotResponse(
                            p.DailyOverviewSnapshot.Id.Value,
                            p.DailyOverviewSnapshot.Date,
                            p.DailyOverviewSnapshot.TargetCalories,
                            p.DailyOverviewSnapshot.TargetProteinGrams,
                            p.DailyOverviewSnapshot.TargetFatGrams,
                            p.DailyOverviewSnapshot.TargetCarbohydrateGrams,
                            p.DailyOverviewSnapshot.ConsumedCalories,
                            p.DailyOverviewSnapshot.BurnedCalories,
                            p.DailyOverviewSnapshot.NetCalories,
                            p.DailyOverviewSnapshot.RemainingCalories,
                            p.DailyOverviewSnapshot.ConsumedProteinGrams,
                            p.DailyOverviewSnapshot.ConsumedFatGrams,
                            p.DailyOverviewSnapshot.ConsumedCarbohydrateGrams,
                            p.DailyOverviewSnapshot.MealCount,
                            p.DailyOverviewSnapshot.ExerciseCount),
                    p.GoalProgressSnapshot == null
                        ? null
                        : new GoalProgressSnapshotResponse(
                            p.GoalProgressSnapshot.Id.Value,
                            p.GoalProgressSnapshot.UserGoalId.Value,
                            p.GoalProgressSnapshot.GoalType.ToString(),
                            p.GoalProgressSnapshot.GoalStartDate,
                            p.GoalProgressSnapshot.GoalTargetDate,
                            p.GoalProgressSnapshot.SnapshotDate,
                            p.GoalProgressSnapshot.StartWeightKg,
                            p.GoalProgressSnapshot.CurrentWeightKg,
                            p.GoalProgressSnapshot.TargetWeightKg,
                            p.GoalProgressSnapshot.Points.Select(x => new GoalProgressSnapshotPointResponse(x.Date, x.WeightKg)).ToList())
                )).ToList(),
                profile.Achievements.Select(a => new SocialProfileAchievementResponse(a.Name, a.Icon)).ToList())),
            errors => Problem(errors));
    }

    [Authorize]
    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFeedQuery(GetUserId().Value, page, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            feed => Ok(ToResponse(feed)),
            errors => Problem(errors)
        );
    }

    [Authorize]
    [HttpPost("posts/text")]
    public async Task<IActionResult> CreateTextPost(
        [FromBody] CreateTextPostRequest body,
        CancellationToken cancellationToken = default)
    {
        var cmd = new CreateTextPostCommand(GetUserId().Value, body.Text, body.Visibility);
        await _mediator.Send(cmd, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpPost("posts/achievement/{userAchievementId}")]
    public async Task<IActionResult> ShareAchievement(
        [FromRoute] Guid userAchievementId,
        [FromBody] ShareAchievementPostRequest body,
        CancellationToken cancellationToken = default)
    {
        var cmd = new ShareAchievementPostCommand(GetUserId().Value, userAchievementId, body.Caption, body.Visibility);
        await _mediator.Send(cmd, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpPost("posts/daily-overview/{localDate}")]
    public async Task<IActionResult> ShareDailyOverview(
        [FromRoute] DateOnly localDate,
        [FromBody] ShareDailyOverviewSnapshotRequest body,
        CancellationToken cancellationToken = default)
    {
        var cmd = new ShareDailyOverviewSnapshotCommand(GetUserId().Value, localDate, body.Caption, body.Visibility);
        await _mediator.Send(cmd, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpPost("posts/goal-progress/{userGoalId}")]
    public async Task<IActionResult> ShareGoalProgress(
        [FromRoute] Guid userGoalId,
        [FromBody] ShareGoalProgressSnapshotRequest body,
        CancellationToken cancellationToken = default)
    {
        var cmd = new ShareGoalProgressSnapshotCommand(GetUserId().Value, userGoalId, body.Caption, body.Visibility);
        await _mediator.Send(cmd, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("posts/{id}")]
    public async Task<IActionResult> DeletePost(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteSocialPostCommand(GetUserId().Value, id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }   

    private PagedResponse<SocialPostResponse> ToResponse(PagedResult<SocialPostResult> result)
    {
        return new PagedResponse<SocialPostResponse>(
            result.Items.Select(item => new SocialPostResponse(
                item.Id.Value,
                new SocialPostAuthorResponse(
                    item.Author.Id.Value,
                    item.Author.Username.Value,
                    item.Author.AvatarUrl),
                (int)item.Type,
                (int)item.Visibility,
                item.LocalTime,
                item.Text,
                item.UserAchievement == null ?
                null :
                new UserAchievementResponse(
                    item.UserAchievement!.Id.Value,
                    item.UserAchievement!.UserId.Value,
                    item.UserAchievement!.AchievementId.Value,
                    item.UserAchievement!.Achievement.Key,
                    item.UserAchievement!.Achievement.Title,
                    item.UserAchievement!.Achievement.Description,
                    item.UserAchievement!.Achievement.Points,
                    item.UserAchievement!.Achievement.Category.ToString(),
                    item.UserAchievement!.Achievement.IconName,
                    item.UserAchievement.LocalDateEarned),
                item.DailyOverviewSnapshot == null ?
                null :
                new DailyOverviewSnapshotResponse(
                    Id: item.DailyOverviewSnapshot.Id.Value,
                    Date: item.DailyOverviewSnapshot.Date,
                    TargetCalories: item.DailyOverviewSnapshot.TargetCalories,
                    TargetProteinGrams: item.DailyOverviewSnapshot.TargetProteinGrams,
                    TargetFatGrams: item.DailyOverviewSnapshot.TargetFatGrams,
                    TargetCarbohydrateGrams: item.DailyOverviewSnapshot.TargetCarbohydrateGrams,
                    ConsumedCalories: item.DailyOverviewSnapshot.ConsumedCalories,
                    BurnedCalories: item.DailyOverviewSnapshot.BurnedCalories,
                    NetCalories: item.DailyOverviewSnapshot.NetCalories,
                    RemainingCalories: item.DailyOverviewSnapshot.RemainingCalories,
                    ConsumedProteinGrams: item.DailyOverviewSnapshot.ConsumedProteinGrams,
                    ConsumedFatGrams: item.DailyOverviewSnapshot.ConsumedFatGrams,
                    ConsumedCarbohydrateGrams: item.DailyOverviewSnapshot.ConsumedCarbohydrateGrams,
                    MealCount: item.DailyOverviewSnapshot.MealCount,
                    ExerciseCount: item.DailyOverviewSnapshot.ExerciseCount
                ),
                item.GoalProgressSnapshot == null ?
                null :
                new GoalProgressSnapshotResponse(
                    Id: item.GoalProgressSnapshot.Id.Value,
                    UserGoalId: item.GoalProgressSnapshot.UserGoalId.Value,
                    GoalType: item.GoalProgressSnapshot.GoalType.ToString(),
                    GoalStartDate: item.GoalProgressSnapshot.GoalStartDate,
                    GoalTargetDate: item.GoalProgressSnapshot.GoalTargetDate,
                    SnapshotDate: item.GoalProgressSnapshot.SnapshotDate,
                    StartWeightKg: item.GoalProgressSnapshot.StartWeightKg,
                    CurrentWeightKg: item.GoalProgressSnapshot.CurrentWeightKg,
                    TargetWeightKg: item.GoalProgressSnapshot.TargetWeightKg,
                    Points: item.GoalProgressSnapshot.Points.Select(p => new GoalProgressSnapshotPointResponse(p.Date, p.WeightKg)).ToList()
                )
                )).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize);
    }
}


public sealed record CreateTextPostRequest(string Text, int? Visibility);
public sealed record ShareAchievementPostRequest(string? Caption, int? Visibility);
public sealed record ShareDailyOverviewSnapshotRequest(string? Caption, int? Visibility);
public sealed record ShareGoalProgressSnapshotRequest(string? Caption, int? Visibility);