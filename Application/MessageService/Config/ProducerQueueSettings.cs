namespace MessageService.Config;

public sealed record ProducerQueueSettings(
    string QueueName,
    string ExchangeName,
    string RoutingKey
);