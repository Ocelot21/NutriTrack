using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriTrack.Notifications.Application.Common.Interfaces.Services;
using NutriTrack.Notifications.Infrastructure.Persistence;
using NutriTrack.Notifications.Infrastructure.Services;

namespace NutriTrack.Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        return services;
    }
}
