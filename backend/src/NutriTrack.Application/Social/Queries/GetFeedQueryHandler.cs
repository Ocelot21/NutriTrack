using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Social.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Social.Queries;

public sealed class GetFeedQueryHandler : IRequestHandler<GetFeedQuery, ErrorOr<PagedResult<SocialPostResult>>>
{
    private readonly ISocialPostRepository _socialPostRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBlobStorageService _blobStorageService;

    public GetFeedQueryHandler(
        ISocialPostRepository socialPostRepository,
        IUserRepository userRepository,
        IBlobStorageService blobStorageService)
    {
        _socialPostRepository = socialPostRepository;
        _userRepository = userRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<PagedResult<SocialPostResult>>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

        var feed = await _socialPostRepository.GetFeedAsync(page, pageSize, cancellationToken);

        var authorIds = feed.Items.Select(p => p.UserId).Distinct().ToList();
        var authors = new Dictionary<UserId, User?>();
        foreach (var authorId in authorIds)
        {
            var user = await _userRepository.GetByIdAsync(authorId, cancellationToken);
            authors[authorId] = user;
        }

        var results = feed.Items.Select(p =>
        {
            var author = authors[p.UserId]!;
            var avatarUrl = string.IsNullOrWhiteSpace(author.AvatarUrl)
                ? null
                : _blobStorageService.GenerateReadUri(BlobContainer.Avatars, author.AvatarUrl);

            return new SocialPostResult(
                Id: p.Id,
                Author: new SocialPostAuthorResult(
                    author.Id,
                    author.Username,
                    avatarUrl),
                Type: p.Type,
                Visibility: p.Visibility,
                LocalTime: p.CreatedAtUtc,
                Text: p.Text,
                UserAchievement: p.UserAchievement,
                DailyOverviewSnapshot: p.DailyOverviewSnapshot,
                GoalProgressSnapshot: p.GoalProgressSnapshot
            );
        }).ToList();

        var paged = new PagedResult<SocialPostResult>(results, feed.TotalCount, feed.Page, feed.PageSize);
        return paged;
    }
}
