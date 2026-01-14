using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Social;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Social.Commands.DeleteSocialPost;

public sealed class DeleteSocialPostCommandHandler : IRequestHandler<DeleteSocialPostCommand, ErrorOr<Unit>>
{
    private readonly ISocialPostRepository _socialPostRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSocialPostCommandHandler(
        ISocialPostRepository socialPostRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _socialPostRepository = socialPostRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteSocialPostCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var postId = new SocialPostId(request.PostId);

        var userExists = await _userRepository.ExistsAsync(userId, cancellationToken);
        if (!userExists)
        {
            return Errors.Users.NotFound;
        }

        var post = await _socialPostRepository.GetByIdAsync(postId, cancellationToken);
        if (post is null)
        {
            return Errors.SocialPosts.NotFound;
        }

        if (post.UserId != userId)
        {
            return Errors.SocialPosts.Forbidden;
        }

        _socialPostRepository.Remove(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
