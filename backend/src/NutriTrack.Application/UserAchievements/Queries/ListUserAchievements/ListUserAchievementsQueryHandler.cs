using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.UserAchievements.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserAchievements.Queries.ListUserAchievements;

public class ListUserAchievementsQueryHandler : IRequestHandler<ListUserAchievementsQuery, ErrorOr<PagedResult<UserAchievementResult>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAchievementRepository _userAchievementRepository;

    public ListUserAchievementsQueryHandler(IUserRepository userRepository, IUserAchievementRepository userAchievementRepository)
    {
        _userRepository = userRepository;
        _userAchievementRepository = userAchievementRepository;
    }


    public async Task<ErrorOr<PagedResult<UserAchievementResult>>> Handle(
        ListUserAchievementsQuery request,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId) is not User user)
        {
            return Errors.Users.NotFound;
        }

        var userAchievements = await _userAchievementRepository.GetPagedAsync(
            request.UserId,
            request.page,
            request.pageSize,
            cancellationToken);

        return new PagedResult<UserAchievementResult>(userAchievements.Items.Select(ua => new UserAchievementResult(
            ua.Id,
            ua.UserId,
            ua.AchievementId,
            ua.Achievement.Key,
            ua.Achievement.Title,
            ua.Achievement.Description,
            ua.Achievement.Points,
            ua.Achievement.Category,
            ua.Achievement.IconName,
            ua.LocalDateEarned)).ToList(),
            userAchievements.TotalCount,
            userAchievements.Page,
            userAchievements.PageSize);
    }
}