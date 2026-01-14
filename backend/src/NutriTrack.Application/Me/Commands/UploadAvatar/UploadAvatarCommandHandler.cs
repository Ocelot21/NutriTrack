using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Domain.Common.Primitives;

namespace NutriTrack.Application.Me.Commands.UploadAvatar;

public sealed class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public UploadAvatarCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<Unit>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        var oldBlobName = user.AvatarUrl;

        var newBlobName = await _blobStorageService.UploadAsync(
            BlobContainer.Avatars,
            request.Image,
            request.FileName,
            request.ContentType,
            cancellationToken);

        user.UpdateProfile(
            Optional<string>.None(),
            Optional<string>.None(),
            Optional<string>.None(),
            newBlobName,
            Optional<string>.None(),
            Optional<string>.None());

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _blobStorageService.DeleteAsync(BlobContainer.Avatars, oldBlobName, cancellationToken);

        return Unit.Value;
    }
}
