using RabbitMQ.Client;

namespace Consumer.RabbitMq;

internal static class RabbitMqConnection
{
    public static async Task<IConnection> ConnectWithRetryAsync(
        ConnectionFactory factory,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                return await factory.CreateConnectionAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"RabbitMQ is not ready yet: {exception.Message}");
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }

        throw new OperationCanceledException(cancellationToken);
    }
}
