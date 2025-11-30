using NutriTrack.Api.Common.Mappings;

namespace NutriTrack.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddMappings();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });
        return services;
    }
}
