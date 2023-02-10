namespace Mango.MessageBus;

public class BaseMessage
{
    protected string Id { get; set; }
    public DateTime MessageCreated { get; set; }
}