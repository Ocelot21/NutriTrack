using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NutriTrack.Application.Common.Interfaces.Authorization;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Infrastructure.Authorization;
using NutriTrack.Infrastructure.Persistence.Repositories;
using NutriTrack.Infrastructure.Persistence.Seed;
using NutriTrack.Infrastructure.Services;

namespace NutriTrack.Infrastructure.Persistence;

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

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var dbSettings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;

            options.UseSqlServer(dbSettings.ConnectionString);

            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IGroceryRepository, GroceryRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IMealRepository, MealRepository>();
        services.AddScoped<IUserExerciseLogRepository, UserExerciseLogRepository>();
        services.AddScoped<IPermissionProvider, PermissionProvider>();
        services.AddScoped<IActivityLevelHistoryRepository, ActivityLevelHistoryRepository>();
        services.AddScoped<IUserGoalRepository, UserGoalRepository>();
        services.AddScoped<IWeightHistoryRepository, WeightHistoryRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();

        services.Scan(scan => scan
                .FromAssemblyOf<ISeeder>()
                .AddClasses(c => c.AssignableTo<ISeeder>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddScoped<SeedRunner>();

        return services;
    }
}