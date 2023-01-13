namespace MessageService.Models;

public delegate void ReceivedEventHandler<in T>(T data);
