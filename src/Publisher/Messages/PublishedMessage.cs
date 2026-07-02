namespace Publisher.Messages;

internal sealed record PublishedMessage(string Id, string Text, DateTimeOffset CreatedAtUtc)
{
    public static PublishedMessage CreateRandom()
    {
        return new PublishedMessage(
            Id: Guid.NewGuid().ToString("N"),
            Text: $"Random message {Random.Shared.Next(100000, 999999)}",
            CreatedAtUtc: DateTimeOffset.UtcNow);
    }
}
