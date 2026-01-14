using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Achievements;
using NutriTrack.Domain.Social;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Social.Commands.ShareAchievementPost;

public sealed class ShareAchievementPostCommandHandler : IRequestHandler<ShareAchievementPostCommand, ErrorOr<Unit>>
{
    private readonly ISocialPostRepository _socialPostRepository;
    private readonly IUserAchievementRepository _userAchievementRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShareAchievementPostCommandHandler(
        ISocialPostRepository socialPostRepository,
        IUserAchievementRepository userAchievementRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _socialPostRepository = socialPostRepository;
        _userAchievementRepository = userAchievementRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(ShareAchievementPostCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var userAchievementId = new UserAchievementId(request.UserAchievementId);

        var userExists = await _userRepository.ExistsAsync(userId, cancellationToken);
        if (!userExists)
        {
            return Errors.Users.NotFound;
        }

        var visibility = request.Visibility.HasValue
            ? (PostVisibility)request.Visibility.Value
            : PostVisibility.Public;

        var exists = await _userAchievementRepository.ExistsAsync(userAchievementId, cancellationToken);
        if (!exists)
        {
            return Errors.Users.NotFound;
        }

        var post = SocialPost.ShareAchievement(userId, userAchievementId, request.Caption, visibility);

        await _socialPostRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
