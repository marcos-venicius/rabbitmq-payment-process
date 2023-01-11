using MessageService.Config;
using MessageService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MessageService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IHost ConfigureMessageService<T>(
        this IHost host,
        ProducerConnectionSettings connectionSettings,
        ProducerQueueSettings queueSettings) where  T : BaseMessageData
    {
        using var scope = host.Services.CreateScope();
        
        var messageService = scope.ServiceProvider.GetRequiredService<IProducer<T>>();
        
        messageService.Configure(connectionSettings, queueSettings);
        
        return host;
    }
}