using DotNetEnv;
using NutriTrack.Api;
using NutriTrack.Api.Reports;
using NutriTrack.Api.Startup;
using NutriTrack.Application;
using NutriTrack.Infrastructure;
using System.IO;

DotNetEnv.Env.Load(Path.Combine("src", "NutriTrack.Api", ".env"));

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

    builder.Services.AddHostedService<ReportRunProcessor>();
}

var app = builder.Build();
{
    await app.MigrateAndSeedAsync();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.MapOpenApi();
    }

    app.UseExceptionHandler("/error");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseCors("AllowAll");
    app.Run();
}