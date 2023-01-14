using Api.Models;
using MessageService;
using MessageService.Config;

using var consumer = new Consumer<MessageData>();

consumer.Configure(
    new ProducerConnectionSettings("localhost", "user", "password", 5672),
    new ProducerQueueSettings("logs-queue", "payments-exchange", "payment.log")
);

consumer.OnReceived += data =>
{
    try
    {
        Console.WriteLine($"[INFO]\t\t [{data.Id}] \t\tPAYMENT OF R$ \"{data.Price / 100.0}\" HAS {data.Message}");

        Thread.Sleep(500);

        var random = new Random();

        var randomNumber = random.Next();

        if (randomNumber % 3 == 0)
            throw new Exception("SOME FAIL");

        Console.WriteLine($"[SUCCESS]\t [{data.Id}] ACK \t\t{DateTime.Now}");
    }
    catch
    {
        Console.WriteLine($"[FAIL]\t\t [{data.Id}] NACK \t\t{DateTime.Now}");
        throw;
    }
    finally
    {
        Console.WriteLine();
    }
};

consumer.StartListen();

Console.WriteLine("press [enter] to quit");
Console.ReadKey();