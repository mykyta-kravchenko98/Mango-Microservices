namespace Mango.Services.PaymentAPI.Messaging.Interfaces;

public interface IRabbitMqConsumer
{
    Task Start();
    Task Stop();
}