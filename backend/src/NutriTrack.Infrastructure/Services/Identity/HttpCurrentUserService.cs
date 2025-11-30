using Microsoft.AspNetCore.Http;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Services.Identity;

public sealed class HttpCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserService(IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
    }

    public UserId? UserId =>
        Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value, out var id)
            ? new UserId(id)
            : null;
}