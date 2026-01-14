using NutriTrack.Domain.Achievements;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Social.Snapshots;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.Social;

public sealed class SocialPost : AggregateRoot<SocialPostId>
{
    private SocialPost() { } // EF

    private SocialPost(
        SocialPostId id,
        UserId userId,
        PostType type,
        PostVisibility visibility,
        string? text,
        UserAchievementId? userAchievementId,
        DailyOverviewSnapshotId? dailyOverviewSnapshotId,
        GoalProgressSnapshotId? goalProgressSnapshotId)
    {
        Id = id;
        UserId = userId;
        Type = type;
        Visibility = visibility;

        Text = text;
        UserAchievementId = userAchievementId;
        DailyOverviewSnapshotId = dailyOverviewSnapshotId;
        GoalProgressSnapshotId = goalProgressSnapshotId;

        ValidateInvariants();
    }

    public UserId UserId { get; private set; } = default!;

    public PostType Type { get; private set; }
    public PostVisibility Visibility { get; private set; } = PostVisibility.Public;

    public string? Text { get; private set; }

    public UserAchievementId? UserAchievementId { get; private set; }
    public UserAchievement? UserAchievement { get; private set; }

    public DailyOverviewSnapshotId? DailyOverviewSnapshotId { get; private set; }
    public DailyOverviewSnapshot? DailyOverviewSnapshot { get; private set; }


    public GoalProgressSnapshotId? GoalProgressSnapshotId { get; private set; }
    public GoalProgressSnapshot? GoalProgressSnapshot { get; private set; }

    public static SocialPost CreateText(
        UserId userId,
        string text,
        PostVisibility visibility = PostVisibility.Public)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text post cannot be empty.", nameof(text));

        text = text.Trim();

        return new SocialPost(
            id: new SocialPostId(Guid.NewGuid()),
            userId: userId,
            type: PostType.Text,
            visibility: visibility,
            text: text,
            userAchievementId: null,
            dailyOverviewSnapshotId: null,
            goalProgressSnapshotId: null);
    }

    public static SocialPost ShareAchievement(
        UserId userId,
        UserAchievementId userAchievementId,
        string? caption = null,
        PostVisibility visibility = PostVisibility.Public)
    {
        caption = caption?.Trim();

        return new SocialPost(
            id: new SocialPostId(Guid.NewGuid()),
            userId: userId,
            type: PostType.AchievementShare,
            visibility: visibility,
            text: string.IsNullOrWhiteSpace(caption) ? null : caption,
            userAchievementId: userAchievementId,
            dailyOverviewSnapshotId: null,
            goalProgressSnapshotId: null);
    }

    public static SocialPost ShareDailyOverviewSnapshot(
        UserId userId,
        DailyOverviewSnapshotId dailyOverviewSnapshotId,
        string? caption = null,
        PostVisibility visibility = PostVisibility.Public)
    {
        caption = caption?.Trim();
        var post = new SocialPost(
            id: new SocialPostId(Guid.NewGuid()),
            userId: userId,
            type: PostType.DailyOverviewShare,
            visibility: visibility,
            text: string.IsNullOrWhiteSpace(caption) ? null : caption,
            userAchievementId: null,
            dailyOverviewSnapshotId: dailyOverviewSnapshotId,
            goalProgressSnapshotId: null);
        return post;
    }

    public static SocialPost ShareGoalProgressSnapshot(
        UserId userId,
        GoalProgressSnapshotId goalProgressSnapshotId,
        string? caption = null,
        PostVisibility visibility = PostVisibility.Public)
    {
        caption = caption?.Trim();
        var post = new SocialPost(
            id: new SocialPostId(Guid.NewGuid()),
            userId: userId,
            type: PostType.GoalProgressShare,
            visibility: visibility,
            text: string.IsNullOrWhiteSpace(caption) ? null : caption,
            userAchievementId: null,
            dailyOverviewSnapshotId: null,
            goalProgressSnapshotId: goalProgressSnapshotId);
        return post;
    }

    private void ValidateInvariants()
    {
        switch (Type)
        {
            case PostType.Text:
                if (string.IsNullOrWhiteSpace(Text))
                    throw new InvalidOperationException("Text post must have Text.");
                if (UserAchievementId is not null)
                    throw new InvalidOperationException("Text post must not reference achievement.");
                break;

            case PostType.AchievementShare:
                if (UserAchievementId is null)
                    throw new InvalidOperationException("AchievementShare must reference UserAchievementId.");
                break;

            default:
                break;
        }
    }
}