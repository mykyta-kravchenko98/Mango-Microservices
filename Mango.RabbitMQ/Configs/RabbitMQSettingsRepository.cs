using Mango.RabbitMQ.Configs.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Mango.RabbitMQ.Configs;

public class RabbitMQSettingsRepository : IRabbitMQSettings
{
    private readonly IConfiguration _configuration;
    
    public string CheckoutMessageQueueName => _configuration["RabbitMQSettings:CheckoutMessageQueueName"];
    public string PaymentMessageQueueName => _configuration["RabbitMQSettings:PaymentMessageQueueName"];
    public string PaymentUpdateMessageQueueName => _configuration["RabbitMQSettings:PaymentStatusUpdateMessageQueueName"];
    public string HostName => _configuration["RabbitMQSettings:HostName"];
    public string UserName => _configuration["RabbitMQSettings:UserName"];
    public string Password => _configuration["RabbitMQSettings:Password"];
    public string VirtualHost => _configuration["RabbitMQSettings:VirtualHost"];

    public RabbitMQSettingsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}