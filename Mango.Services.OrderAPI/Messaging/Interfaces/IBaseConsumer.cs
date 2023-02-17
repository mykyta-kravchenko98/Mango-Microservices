namespace Mango.Services.OrderAPI.Messaging.Interfaces;

public interface IBaseConsumer
{
    Task Start();
    Task Stop();
}