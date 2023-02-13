namespace Mango.MessageBus;

public class BaseMessage
{
    protected BaseMessage()
    {
        Id = Guid.NewGuid().ToString();
        MessageCreated = DateTime.Now;
    }
    protected string Id { get; set; }
    public DateTime MessageCreated { get; set; }
}