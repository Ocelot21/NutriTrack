using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Social.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Social.Queries.GetSocialProfile;

public sealed class GetSocialProfileQueryHandler : IRequestHandler<GetSocialProfileQuery, ErrorOr<SocialProfileResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAchievementRepository _userAchievementRepository;
    private readonly ISocialPostRepository _socialPostRepository;
    private readonly IBlobStorageService _blobStorageService;

    public GetSocialProfileQueryHandler(
        IUserRepository userRepository,
        IUserAchievementRepository userAchievementRepository,
        ISocialPostRepository socialPostRepository,
        IBlobStorageService blobStorageService)
    {
        _userRepository = userRepository;
        _userAchievementRepository = userAchievementRepository;
        _socialPostRepository = socialPostRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<SocialProfileResult>> Handle(GetSocialProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        string? avatarUrl = null;
        if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
        {
            avatarUrl = _blobStorageService.GenerateReadUri(BlobContainer.Avatars, user.AvatarUrl);
        }

        var totalPoints = await _userAchievementRepository.SumPointsAsync(user.Id, cancellationToken);

        var achievementsPaged = await _userAchievementRepository.GetPagedAsync(user.Id, page: 1, pageSize: 100, cancellationToken);
        var achievements = achievementsPaged.Items
            .Select(a => new SocialProfileAchievementResult(
                Name: a.Achievement.Title,
                Icon: a.Achievement.IconName))
            .ToList();

        var posts = await _socialPostRepository.ListByUserAsync(user.Id, take: 50, cancellationToken);
        var postResults = posts.Select(p => new SocialPostResult(
            Id: p.Id,
            Author: new SocialPostAuthorResult(user.Id, user.Username, avatarUrl),
            Type: p.Type,
            Visibility: p.Visibility,
            LocalTime: p.CreatedAtUtc,
            Text: p.Text,
            UserAchievement: p.UserAchievement,
            DailyOverviewSnapshot: p.DailyOverviewSnapshot,
            GoalProgressSnapshot: p.GoalProgressSnapshot)).ToList();

        return new SocialProfileResult(
            UserId: user.Id.Value,
            Username: user.Username.Value,
            AvatarUrl: avatarUrl,
            TotalPoints: totalPoints,
            Posts: postResults,
            Achievements: achievements);
    }
}
