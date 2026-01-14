using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Application.Me.Queries.GetMe;

public sealed class GetMeQueryHandler : IRequestHandler<GetMeQuery, ErrorOr<UserResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IBlobStorageService _blobStorageService;

    public GetMeQueryHandler(IUserRepository userRepository, IBlobStorageService blobStorageService)
    {
        _userRepository = userRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<UserResult>> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        var result = user.ToUserResult();
        return result with
        {
            AvatarUrl = _blobStorageService.GenerateReadUri(BlobContainer.Avatars, result.AvatarUrl)
        };
    }
}
