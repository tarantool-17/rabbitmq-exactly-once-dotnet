using Publisher.Messages;
using Publisher.RabbitMq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var rabbitMqOptions = RabbitMqOptions.FromEnvironment();
await using var rabbitMqPublisher = new RabbitMqPublisher(rabbitMqOptions, app.Logger);
await rabbitMqPublisher.StartAsync(app.Lifetime.ApplicationStopping);

app.MapPost("/messages", async (CancellationToken cancellationToken) =>
{
    var message = PublishedMessage.CreateRandom();
    await rabbitMqPublisher.PublishAsync(message, cancellationToken);

    return Results.Accepted(value: message);
});

app.Logger.LogInformation("Publisher API connected to RabbitMQ host '{RabbitMqHost}'.", rabbitMqOptions.HostName);

await app.RunAsync();
