using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NutriTrack.Notifications.Application;
using NutriTrack.Notifications.Infrastructure;
using NutriTrack.Notifications.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // tvoj AddApplication / AddInfrastructure koje si ve? napravio
        services.AddApplication().AddInfrastructure(configuration);

        // RabbitMQ config
        services.Configure<RabbitMqSettings>(
            configuration.GetSection(RabbitMqSettings.SectionName));

        // hosted worker
        services.AddHostedService<NotificationConsumerWorker>();
    })
    .Build();

await host.RunAsync();
