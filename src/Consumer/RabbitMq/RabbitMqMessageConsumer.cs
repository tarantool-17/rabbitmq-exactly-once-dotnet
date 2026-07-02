using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.RabbitMq;

internal sealed class RabbitMqMessageConsumer : IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqMessageConsumer(RabbitMqOptions options)
    {
        _options = options;
    }

    public async Task StartAsync(Func<string, Task> handleMessageAsync, CancellationToken cancellationToken)
    {
        _connection = await RabbitMqConnection.ConnectWithRetryAsync(
            _options.CreateConnectionFactory(),
            cancellationToken);

        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
            await handleMessageAsync(message);
        };

        await _channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: true,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}
