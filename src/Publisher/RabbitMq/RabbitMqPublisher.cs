using System.Text.Json;
using Publisher.Messages;
using RabbitMQ.Client;

namespace Publisher.RabbitMq;

internal sealed class RabbitMqPublisher : IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger _logger;
    private IConnection? _connection;

    public RabbitMqPublisher(RabbitMqOptions options, ILogger logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = await RabbitMqConnection.ConnectWithRetryAsync(
            _options.CreateConnectionFactory(),
            _logger,
            cancellationToken);

        await using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
    }

    public async Task PublishAsync(PublishedMessage message, CancellationToken cancellationToken)
    {
        if (_connection is null)
        {
            throw new InvalidOperationException("RabbitMQ publisher was not started.");
        }

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        await using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _options.QueueName,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Published message {MessageId}", message.Id);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}
