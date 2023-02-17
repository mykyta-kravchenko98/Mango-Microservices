using Mango.Kafka.Configs.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Mango.Kafka.Configs;

public class KafkaSettingsRepository : IKafkaSettings
{
    private readonly IConfiguration _configuration;
    
    public string CheckoutMessageTopicName => _configuration["KafkaSettings:CheckoutMessageTopicName"];
    public string PaymentMessageTopicName => _configuration["KafkaSettings:PaymentMessageTopicName"];
    public string PaymentUpdateMessageTopicName => _configuration["KafkaSettings:PaymentUpdateMessageTopicName"];
    public string BootstrapServers => _configuration["KafkaSettings:BootstrapServers"];
    
    public string CheckoutGroupId => _configuration["KafkaSettings:CheckoutGroupId"];
    public string PaymentGroupId => _configuration["KafkaSettings:PaymentGroupId"];
    public string PaymentStatusUpdateGroupId => _configuration["KafkaSettings:PaymentStatusUpdateGroupId"];

    public KafkaSettingsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}