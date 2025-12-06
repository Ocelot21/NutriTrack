using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NutriTrack.Application.Common.Interfaces.Messaging;
using NutriTrack.Application.Notifications.Messages;
using RabbitMQ.Client;

namespace NutriTrack.Infrastructure.Messaging;

public sealed class RabbitMqNotificationPublisher : INotificationPublisher, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly ConnectionFactory _factory;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqNotificationPublisher(IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;

        _factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };
    }

    private async Task EnsureChannelAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
            return;

        _connection = await _factory.CreateConnectionAsync(cancellationToken: cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
    }

    public async Task PublishAsync(
        NotificationRequestedMessage message,
        CancellationToken cancellationToken = default)
    {
        await EnsureChannelAsync(cancellationToken);

        if (_channel is null)
        {
            throw new InvalidOperationException("RabbitMQ channel not initialized.");
        }

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: _settings.QueueName,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}
