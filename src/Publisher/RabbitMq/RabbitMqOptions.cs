using RabbitMQ.Client;

namespace Publisher.RabbitMq;

internal sealed record RabbitMqOptions(
    string HostName,
    string UserName,
    string Password,
    string QueueName)
{
    public static RabbitMqOptions FromEnvironment()
    {
        return new RabbitMqOptions(
            HostName: Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            UserName: Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
            Password: Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest",
            QueueName: Environment.GetEnvironmentVariable("RABBITMQ_QUEUE") ?? "message");
    }

    public ConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = HostName,
            UserName = UserName,
            Password = Password
        };
    }
}
