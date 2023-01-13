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
        Console.WriteLine($"* [{data.Id}] MAKING PAYMENT OF R$ {data.Price / 100.0}");
        Thread.Sleep(500);
        Console.WriteLine($"+ [{data.Id}] PAYMENT MAKE");
        
        var random = new Random();

        var randomNumber = random.Next();

        if (randomNumber % 3 == 0)
            throw new Exception("SOME FAIL");

        logPaymentProducer.Publish(new MessageData(data.Price, "SUCCESS"));
    }
    catch
    {
        logPaymentProducer.Publish(new MessageData(data.Price, "FAIL"));
        
        throw;
    }
};

makePaymentQueueConsumer.StartListen();

Console.WriteLine("press [enter] to quit");
Console.ReadKey();