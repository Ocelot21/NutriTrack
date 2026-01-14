using NutriTrack.Api.Common.Clients;
using NutriTrack.Api.Common.Mappings;
using System.Net.Http.Headers;

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
        //services.AddOpenApi();

        services.AddHttpClient<NotificationsClient>((sp, http) =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            var baseUrl = cfg["Services:Notifications:BaseUrl"] ?? throw new Exception("Notifications BaseUrl missing");
            var apiKey = cfg["Services:Notifications:ApiKey"] ?? throw new Exception("Notifications ApiKey missing");

            http.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            http.DefaultRequestHeaders.Add("X-Internal-Api-Key", apiKey);
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });


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
