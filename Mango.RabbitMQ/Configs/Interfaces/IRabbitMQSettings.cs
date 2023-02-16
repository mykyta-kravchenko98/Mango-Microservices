namespace Mango.RabbitMQ.Configs.Interfaces;

public interface IRabbitMQSettings
{
    //string AzureServiceBusConnectionUrl { get; }
    string CheckoutMessageQueueName { get; }
    string PaymentMessageQueueName { get; }
    string PaymentUpdateMessageQueueName { get; }
    string HostName { get; }
    string UserName { get; }
    string Password { get; }
    string VirtualHost { get; }
}