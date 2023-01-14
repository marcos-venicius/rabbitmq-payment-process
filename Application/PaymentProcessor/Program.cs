using Api.Models;
using MessageService;
using MessageService.Config;

using var makePaymentQueueConsumer = new Consumer<MessageData>();
using var logPaymentProducer = new Producer<MessageData>();

var producerConnection = new ProducerConnectionSettings("localhost", "user", "password", 5672);

logPaymentProducer.Configure(
    producerConnection,
    new ProducerQueueSettings("logs-queue", "payments-exchange", "payment.log")
);

makePaymentQueueConsumer.Configure(
    producerConnection,
    new ProducerQueueSettings("payments-queue", "payments-exchange", "payment.make")
);

makePaymentQueueConsumer.OnReceived += data =>
{
    try
    {
        Console.WriteLine($"[INFO]\t\t [{data.Id}] {DateTime.Now} PAYMENT OF R$ {data.Price / 100.0}");

        Thread.Sleep(2_000); // 2 seconds


        var random = new Random();

        var randomNumber = random.Next();

        if (randomNumber % 3 == 0)
            throw new Exception("SOME FAIL");

        Console.WriteLine($"[SUCCESS]\t [{data.Id}] {DateTime.Now} PAID");

        logPaymentProducer.Publish(new MessageData(data.Id, data.Price, "SUCCESS"));
    }
    catch
    {
        Console.WriteLine($"[FAIL]\t\t [{data.Id}] {DateTime.Now} NOT PAID");

        logPaymentProducer.Publish(new MessageData(data.Id, data.Price, "FAIL"));

        throw;
    }
    finally
    {
        Console.WriteLine();
    }
};

makePaymentQueueConsumer.StartListen();

Console.WriteLine("press [enter] to quit");
Console.ReadKey();