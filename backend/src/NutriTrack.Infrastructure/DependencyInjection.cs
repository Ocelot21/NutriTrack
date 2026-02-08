using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriTrack.Application.AdminDashboard;
using NutriTrack.Application.Common.Interfaces.Messaging;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Security;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Reports.Services;
using NutriTrack.Domain.Common.Events;
using NutriTrack.Infrastructure.Authentication;
using NutriTrack.Infrastructure.AdminDashboard;
using NutriTrack.Infrastructure.Messaging;
using NutriTrack.Infrastructure.Persistence;
using NutriTrack.Infrastructure.Persistence.Repositories;
using NutriTrack.Infrastructure.Reports.Data;
using NutriTrack.Infrastructure.Reports.Pdf;
using NutriTrack.Infrastructure.Reports.Storage;
using NutriTrack.Infrastructure.Security;
using NutriTrack.Infrastructure.Services;
using NutriTrack.Infrastructure.Services.Achievements;
using NutriTrack.Infrastructure.Services.Groceries;
using NutriTrack.Infrastructure.Services.Identity;
using NutriTrack.Infrastructure.Storage.Blob;

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
        services.AddBlobStorage(configuration);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ITimeZoneService, TimeZoneService>();
        services.AddScoped<ICurrentUserService, HttpCurrentUserService>();

        services.AddScoped<IReportDataService, ReportDataService>();
        services.AddScoped<IReportPdfGenerator, QuestPdfReportPdfGenerator>();
        services.AddScoped<IReportOutputStorage, ReportOutputStorage>();

        services.Configure<RabbitMqSettings>(
            configuration.GetSection(RabbitMqSettings.SectionName));
        services.AddSingleton<INotificationPublisher, RabbitMqNotificationPublisher>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IGroceryRecommender, GroceryRecommender>();
        services.AddScoped<IEnhancedGroceryRecommender, EnhancedGroceryRecommender>();
        services.AddScoped<IMealItemReadRepository, MealItemReadRepository>();
        services.AddScoped<IRecommendationMetricsTracker, RecommendationMetricsTracker>();

        services.AddMemoryCache();
        services.AddHostedService<RecommendationCacheWarmer>();

        services.AddScoped<IAdminDashboardService, AdminDashboardService>();

        services.AddDataProtection();
        services.AddScoped<ITotpSecretProtector, TotpSecretProtector>();

        return services;
    }
}