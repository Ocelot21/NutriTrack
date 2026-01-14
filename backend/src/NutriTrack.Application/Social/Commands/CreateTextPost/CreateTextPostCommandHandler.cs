using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Social;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Social.Commands.CreateTextPost;

public sealed class CreateTextPostCommandHandler : IRequestHandler<CreateTextPostCommand, ErrorOr<Unit>>
{
    private readonly ISocialPostRepository _socialPostRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTextPostCommandHandler(
        ISocialPostRepository socialPostRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _socialPostRepository = socialPostRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(CreateTextPostCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var userExists = await _userRepository.ExistsAsync(userId, cancellationToken);
        if (!userExists)
        {
            return Errors.Users.NotFound;
        }

        var visibility = request.Visibility.HasValue
            ? (PostVisibility)request.Visibility.Value
            : PostVisibility.Public;

        var post = SocialPost.CreateText(userId, request.Text, visibility);

        await _socialPostRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
