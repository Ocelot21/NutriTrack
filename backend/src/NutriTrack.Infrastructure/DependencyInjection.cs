using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Infrastructure.Authentication;
using NutriTrack.Infrastructure.Persistence;
using NutriTrack.Infrastructure.Services;
using NutriTrack.Infrastructure.Services.Identity;

namespace NutriTrack.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        services.AddAuthentication(configuration);
        services.AddPersistence(configuration);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ITimeZoneService, TimeZoneService>();
        services.AddScoped<ICurrentUserService, HttpCurrentUserService>();

        return services;
    }
}