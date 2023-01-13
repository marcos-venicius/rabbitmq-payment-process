using MessageService.Config;
using MessageService.Models;

namespace MessageService;

public interface IConsumer<out T> : IDisposable where T : BaseMessageData
{
    public void Configure(ProducerConnectionSettings connectionSettings, ProducerQueueSettings queueSettings);
    public void StartListen();
    public event ReceivedEventHandler<T> OnReceived;
}