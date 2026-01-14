using MediatR;
using ErrorOr;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Social.Common;

namespace NutriTrack.Application.Social.Queries;

public sealed record GetFeedQuery(
    Guid UserId,
    int Page,
    int PageSize
) : IRequest<ErrorOr<PagedResult<SocialPostResult>>>;
