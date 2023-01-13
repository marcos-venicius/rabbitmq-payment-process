using System.Text;
using MessageService.Config;
using MessageService.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageService;

public sealed class Consumer<T> : IConsumer<T> where T : BaseMessageData
{
    private IConnection? _connection;
    private IModel? _channel;
    private ProducerQueueSettings? _producerQueueSettings;

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

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

    public void StartListen()
    {
        if (_producerQueueSettings is null || _connection is null || _channel is null)
            throw new ApplicationException("MISSING_CALL_CONFIGURE_METHOD");

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (_, eventArgs) =>
        {
            try
            {
                var bytes = eventArgs.Body.ToArray();
                var serialized = Encoding.UTF8.GetString(bytes);

                var obj = JsonConvert.DeserializeObject<T>(serialized);

                if (obj is null)
                    throw new ApplicationException("CANNOT_CONVERT_DATA");

                OnReceived?.Invoke(obj);

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch
            {
                _channel.BasicNack(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true
                );
            }
        };

        _channel.BasicConsume(
            queue: _producerQueueSettings.QueueName,
            autoAck: false,
            consumer: consumer
        );
    }

    public event ReceivedEventHandler<T>? OnReceived;
}