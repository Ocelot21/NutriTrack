using ErrorOr;
using MediatR;

namespace NutriTrack.Application.Social.Queries.GetSocialProfile;

public sealed record GetSocialProfileQuery(Guid UserId) : IRequest<ErrorOr<SocialProfileResult>>;
