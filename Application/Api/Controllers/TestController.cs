using MessageService;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController, Route("/api/v1/tests")]
public class TestController : ControllerBase
{
    private readonly IProducer<MessageData> _producer;

    public TestController(IProducer<MessageData> producer)
    {
        _producer = producer;
    }

    [HttpPost("publish-message")]
    public void PublishMessage(string message)
    {
        _producer.Publish(new MessageData(message));
    }
}