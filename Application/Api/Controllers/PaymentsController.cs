using Api.DTOs;
using MessageService;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController, Route("/api/v1/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IProducer<MessageData> _makePaymentProducer;

    public PaymentsController(IProducer<MessageData> makePaymentProducer)
    {
        _makePaymentProducer = makePaymentProducer;
    }

    [HttpPost]
    public void MakePayment(PaymentsCreateDto dto)
    {
        _makePaymentProducer.Publish(new MessageData(Guid.NewGuid(), dto.Price, "MAKE PAYMENT"));
    }
}