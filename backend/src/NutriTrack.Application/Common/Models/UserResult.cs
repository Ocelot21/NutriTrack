using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Application.Common.Models
{
    public record UserResult(
        UserId Id,
        Username Username,
        Email Email,
        string FirstName,
        string LastName,
        Role? Role,
        bool IsEmailVerified,
        string? AvatarUrl,
        string TimeZoneId,
        DateTime? LastLoginAtUtc,
        CountryCode? Country,
        bool IsHealthProfileCompleted,
        Gender Gender,
        ActivityLevel ActivityLevel,
        DateOnly? Birthdate,
        decimal? HeightCm,
        decimal? WeightKg
    );
}
