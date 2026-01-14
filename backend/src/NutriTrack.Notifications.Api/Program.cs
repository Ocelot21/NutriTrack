using Microsoft.EntityFrameworkCore;
using NutriTrack.Notifications.Api;
using NutriTrack.Notifications.Application;
using NutriTrack.Notifications.Infrastructure;
using NutriTrack.Notifications.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<InternalApiKeyMiddleware>();

builder.Services.AddApplication().AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<InternalApiKeyMiddleware>();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("ok"));

app.Run();