namespace Mango.MessageBus;

public interface IMessageBusSettings
{
    string AzureServiceBusConnectionUrl { get; }
    string CheckoutMessageTopicName { get; }
    string PaymentMessageTopicName { get; }
}