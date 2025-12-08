using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriTrack.Application.Common.Interfaces.Messaging;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.Common.Events;
using NutriTrack.Infrastructure.Authentication;
using NutriTrack.Infrastructure.Messaging;
using NutriTrack.Infrastructure.Notifications;
using NutriTrack.Infrastructure.Persistence;
using NutriTrack.Infrastructure.Services;
using NutriTrack.Infrastructure.Services.Achievements;
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
        services.AddNotificationsReadServices(configuration);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ITimeZoneService, TimeZoneService>();
        services.AddScoped<ICurrentUserService, HttpCurrentUserService>();
        services.Configure<RabbitMqSettings>(
        configuration.GetSection(RabbitMqSettings.SectionName));
        services.AddSingleton<INotificationPublisher, RabbitMqNotificationPublisher>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}