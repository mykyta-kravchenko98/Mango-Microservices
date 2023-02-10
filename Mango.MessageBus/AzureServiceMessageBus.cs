using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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
            case Topics.CheckoutMessageTopic:
                topicName = _messageBusSettings.CheckoutMessageTopicName;
                break;
            
            default:
                topicName = _messageBusSettings.CheckoutMessageTopicName;
                break;
        }

        await using var client = new ServiceBusClient(_messageBusSettings.AzureServiceBusConnectionUrl);
        ServiceBusSender sender = client.CreateSender(topicName);

        var jsonMessage = JsonConvert.SerializeObject(message);
        ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };

        await sender.SendMessageAsync(finalMessage);
    }
}