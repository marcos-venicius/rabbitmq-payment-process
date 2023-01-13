using MessageService.Models;

namespace Api.Models;

public class MessageData : BaseMessageData
{
    public string Message { get; set; }
    public int Price { get; set; }

    public MessageData(int price, string message) : base(Guid.NewGuid())
    {
        Price = price;
        Message = message;
    }
}