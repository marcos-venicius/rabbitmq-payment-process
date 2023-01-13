using Api.Models;
using MessageService;
using MessageService.Config;

using var consumer = new Consumer<MessageData>();

consumer.Configure(
    new ProducerConnectionSettings("localhost", "user", "password", 5672),
    new ProducerQueueSettings("events-queue", "events-ex", "save-event")
);

consumer.OnReceived += data =>
{
    try
    {
        Console.WriteLine($"* [{data.Id}] SAVING LOG \"{data.Message}\"");
        Thread.Sleep(500);
        
        var random = new Random();

        var randomNumber = random.Next();

        if (randomNumber % 3 == 0)
            throw new Exception("SOME FAIL");
        
        Console.WriteLine($"+ [{data.Id}] LOG SAVED, ACK ");
    }
    catch
    {
        Console.WriteLine($"! [{data.Id}] LOG SAVE FAILED, NACK ");
        throw;
    }
};

consumer.StartListen();

Console.WriteLine("press [enter] to quit");
Console.ReadKey();