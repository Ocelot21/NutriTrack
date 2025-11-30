using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Mappings;

public static class UserMappings
{
    public static UserResult ToUserResult(this User user)
    {
        return new UserResult(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role,
            user.IsEmailVerified,
            user.AvatarUrl,
            user.TimeZoneId,
            user.LastLoginAtUtc,
            user.Country,
            user.IsHealthProfileCompleted,
            user.Gender,
            user.ActivityLevel,
            user.Birthdate,
            user.HeightCm,
            user.WeightKg
        );
    }
}