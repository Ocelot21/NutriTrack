using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Options;
using NutriTrack.Notifications.Application.Notifications.Commands.CreateNotification;
using NutriTrack.Notifications.Domain.Notifications;
using NutriTrack.Shared.Contracts.Notifications;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NutriTrack.Notifications.Worker;

public sealed class NotificationConsumerWorker : BackgroundService
{
    private readonly ILogger<NotificationConsumerWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqSettings _settings;

    private IConnection? _connection;
    private IChannel? _channel;

    public NotificationConsumerWorker(
        ILogger<NotificationConsumerWorker> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqSettings> options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Attempting to connect to RabbitMQ at {HostName}:{Port}", _settings.HostName, _settings.Port);
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                await _channel.QueueDeclareAsync(
                    queue: _settings.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("Successfully connected to RabbitMQ and declared queue {QueueName}", _settings.QueueName);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    var bodyBytes = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(bodyBytes);

                    try
                    {
                        var message = JsonSerializer.Deserialize<NotificationRequestedMessage>(json);

                        if (message is null)
                        {
                            _logger.LogWarning(
                                "Received invalid NotificationRequestedMessage: {Json}",
                                json);

                            await _channel.BasicAckAsync(
                                ea.DeliveryTag,
                                multiple: false,
                                cancellationToken: stoppingToken);

                            return;
                        }

                        using var scope = _scopeFactory.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                        var command = new CreateNotificationCommand(
                            message.UserId,
                            message.Title,
                            message.Message,
                            Enum.Parse<NotificationType>(message.Type),
                            message.OccurredAtUtc,
                            message.LinkUrl,
                            message.MetadataJson);

                        await mediator.Send(command, stoppingToken);

                        await _channel.BasicAckAsync(
                            ea.DeliveryTag,
                            multiple: false,
                            cancellationToken: stoppingToken);

                        _logger.LogInformation(
                            "Processed notification for user {UserId}, type {Type}",
                            message.UserId,
                            message.Type);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error while processing notification message: {Json}",
                            json);

                        await _channel.BasicNackAsync(
                            ea.DeliveryTag,
                            multiple: false,
                            requeue: false,
                            cancellationToken: stoppingToken);
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: _settings.QueueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation(
                    "NotificationConsumerWorker started. Listening on queue {Queue}",
                    _settings.QueueName);

                try
                {
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (TaskCanceledException)
                {

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while connecting to RabbitMQ. Retrying in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken: cancellationToken);
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync(cancellationToken: cancellationToken);
            await _connection.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
