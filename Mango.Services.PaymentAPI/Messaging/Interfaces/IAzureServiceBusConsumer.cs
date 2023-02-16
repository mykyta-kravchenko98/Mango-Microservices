namespace Mango.Services.PaymentAPI.Messaging.Interfaces;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}