namespace Mango.Services.OrderAPI.Messaging.Interfaces;

public interface IRabbitMqConsumer
{
    Task Start();
    Task Stop();
}