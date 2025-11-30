using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Common.Interfaces.Persistence;

namespace NutriTrack.Application.Me.Queries.GetMe;

public sealed class GetMeQueryHandler : IRequestHandler<GetMeQuery, ErrorOr<UserResult>>
{
    private readonly IUserRepository _userRepository;

    public GetMeQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<UserResult>> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        return user.ToUserResult();
    }
}
