using RabbitMQ.Client;

namespace Publisher.RabbitMq;

internal static class RabbitMqConnection
{
    public static async Task<IConnection> ConnectWithRetryAsync(
        ConnectionFactory factory,
        ILogger logger,
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
                logger.LogInformation(exception, "RabbitMQ is not ready yet.");
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }

        throw new OperationCanceledException(cancellationToken);
    }
}
