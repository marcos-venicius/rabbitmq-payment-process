namespace MessageService.Config;

public sealed record ProducerConnectionSettings(
    string? RabbitMqHostname,
    string? RabbitMqUser,
    string? RabbitMqPassword,
    int? RabbitMqPort
);
