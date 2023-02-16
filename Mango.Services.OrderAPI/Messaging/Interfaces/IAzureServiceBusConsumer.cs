namespace Mango.Services.OrderAPI.Messaging.Interfaces;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}