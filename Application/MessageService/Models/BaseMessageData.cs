namespace MessageService.Models;

public abstract class BaseMessageData
{
    public Guid Id { get; }
    
    public BaseMessageData(Guid id)
    {
        Id = id;
    }
}