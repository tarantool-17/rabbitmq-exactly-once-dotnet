using Consumer.RabbitMq;

using var stoppingTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    stoppingTokenSource.Cancel();
};

var rabbitMqOptions = RabbitMqOptions.FromEnvironment();
await using var consumer = new RabbitMqMessageConsumer(rabbitMqOptions);

await consumer.StartAsync(message =>
{
    Console.WriteLine($"Received: {message}");
    return Task.CompletedTask;
}, stoppingTokenSource.Token);

Console.WriteLine($"Consumer connected to RabbitMQ host '{rabbitMqOptions.HostName}'. Waiting for messages.");

try
{
    await Task.Delay(Timeout.InfiniteTimeSpan, stoppingTokenSource.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Consumer is stopping.");
}
