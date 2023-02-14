using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mango.MessageBus;

public class AzureServiceMessageBus : IMessageBus
{
    private readonly IMessageBusSettings _messageBusSettings;
    public AzureServiceMessageBus(IMessageBusSettings messageBusSettings)
    {
        _messageBusSettings = messageBusSettings;
    }
    public async Task PublishMessage(BaseMessage message, Topics topic)
    {
        string topicName = String.Empty;
        switch (topic)
        {
            case Topics.CheckoutMessage:
                topicName = _messageBusSettings.CheckoutMessageTopicName;
                break;
            
            case Topics.PaymentMessage:
                topicName = _messageBusSettings.PaymentMessageTopicName;
                break;
            
            case Topics.PaymentUpdateMessage:
                topicName = _messageBusSettings.PaymentUpdateMessageTopicName;
                break;
            
            default:
                topicName = _messageBusSettings.CheckoutMessageTopicName;
                break;
        }

        await using var client = new ServiceBusClient(_messageBusSettings.AzureServiceBusConnectionUrl);
        ServiceBusSender sender = client.CreateSender(topicName);

        var stringEnumConverter = new StringEnumConverter();
        var jsonMessage = JsonConvert.SerializeObject(message, stringEnumConverter);
        ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };

        await sender.SendMessageAsync(finalMessage);
    }
}