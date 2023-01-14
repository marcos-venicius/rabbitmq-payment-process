using System.Text;
using MessageService.Config;
using MessageService.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MessageService;

public sealed class Producer<T> : IProducer<T> where T : BaseMessageData
{
    private IConnection? _connection;
    private IModel? _channel;
    private ProducerQueueSettings? _producerQueueSettings;

    public void Configure(ProducerConnectionSettings connectionSettings, ProducerQueueSettings queueSettings)
    {
        _producerQueueSettings = queueSettings;
        
        var factory = new ConnectionFactory
        {
            HostName = connectionSettings.RabbitMqHostname ?? "localhost",
            UserName = connectionSettings.RabbitMqUser ?? "user",
            Password = connectionSettings.RabbitMqPassword ?? "password",
            Port = connectionSettings.RabbitMqPort ?? 5672
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare(
            queue: queueSettings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        
        _channel.ExchangeDeclare(
            exchange: queueSettings.ExchangeName,
            type: "direct",
            autoDelete: false,
            arguments: null
        );
        
        _channel.QueueBind(
            queue: queueSettings.QueueName,
            exchange: queueSettings.ExchangeName,
            routingKey: queueSettings.RoutingKey,
            arguments: null
        );
    }

    private static byte[] GetDataBytes(T data)
    {
        var serializeObject = JsonConvert.SerializeObject(data);

        if (serializeObject is null)
            throw new ApplicationException("CANNOT_SERIALIZE_DATA");

        return Encoding.UTF8.GetBytes(serializeObject);
    }

    public void Publish(T data)
    {
        if (_producerQueueSettings is null || _connection is null || _channel is null)
            throw new ApplicationException("MISSING_CALL_CONFIGURE_METHOD");
        
        var body = GetDataBytes(data);

        var properties = _channel.CreateBasicProperties();

        properties.Persistent = true;
        
        _channel.BasicPublish(
            exchange: _producerQueueSettings.ExchangeName,
            routingKey: _producerQueueSettings.RoutingKey,
            basicProperties: properties,
            body: body
        );
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}