using MessageService.Config;
using MessageService.Models;

namespace MessageService;

public interface IProducer<in T> : IDisposable where T : BaseMessageData
{
    public void Configure(ProducerConnectionSettings connectionSettings, ProducerQueueSettings queueSettings);
    public void Publish(T data);
}