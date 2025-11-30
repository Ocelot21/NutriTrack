using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Services;

public interface ICurrentUserService
{
    UserId? UserId { get; }
}
