using MessageService.Models;

namespace Api.Models;

public class MessageData : BaseMessageData
{
    public string Message { get; set; }

    public MessageData(string message) : base(Guid.NewGuid())
    {
        Message = message;
    }
}