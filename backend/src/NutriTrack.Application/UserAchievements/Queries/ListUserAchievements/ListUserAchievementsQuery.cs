using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.UserAchievements.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserAchievements.Queries.ListUserAchievements
{
    public record ListUserAchievementsQuery(
        UserId UserId,
        int page = 1,
        int pageSize = 10) : IRequest<ErrorOr<PagedResult<UserAchievementResult>>>;
}
