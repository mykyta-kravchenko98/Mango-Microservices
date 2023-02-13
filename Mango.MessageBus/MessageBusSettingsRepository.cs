using Microsoft.Extensions.Configuration;

namespace Mango.MessageBus;

public class MessageBusSettingsRepository : IMessageBusSettings
{
    private readonly IConfiguration _configuration;

    public MessageBusSettingsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string AzureServiceBusConnectionUrl => _configuration["AzureMessageBusSettings:AzureServiceBusConnection"];
    public string CheckoutMessageTopicName => _configuration["AzureMessageBusSettings:CheckoutMessageTopicName"];
    public string PaymentMessageTopicName => _configuration["AzureMessageBusSettings:PaymentMessageTopicName"];
}