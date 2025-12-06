using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NutriTrack.Notifications.Application.Common.Interfaces.Persistence;
using NutriTrack.Notifications.Infrastructure.Persistence.Repositories;

namespace NutriTrack.Notifications.Infrastructure.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(
            configuration.GetSection(DatabaseSettings.SectionName));

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

        services.AddDbContext<NotificationsDbContext>((sp, options) =>
        {
            var dbSettings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;

            options.UseSqlServer(dbSettings.ConnectionString);

            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
