using MessageService.Models;

namespace Api.Models;

public class MessageData : BaseMessageData
{
    public string Message { get; set; }
    public int Price { get; set; }

    public MessageData(Guid id, int price, string message) : base(id)
    {
        Price = price;
        Message = message;
    }
}