using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Infrastructure.Notifications.Persistence;

namespace NutriTrack.Infrastructure.Notifications;

public static class NotificationsReadExtensions
{
    public static void AddNotificationsReadServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationsReadDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("NotificationsDatabase")));


        services.AddScoped<IUserNotificationsReadRepository, UserNotificationReadRepository>();
    }
}
