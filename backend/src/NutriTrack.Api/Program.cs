using NutriTrack.Api;
using NutriTrack.Api.Startup;
using NutriTrack.Application;
using NutriTrack.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);
}

var app = builder.Build();
{
    await app.MigrateAndSeedAsync();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseExceptionHandler("/error");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseCors("AllowAll");
    app.Run();
}