using System.Text;
using System.Text.Json;
using Mango.RabbitMQ.Configs.Interfaces;
using Mango.RabbitMQ.Enums;
using Mango.RabbitMQ.Services.Interfaces;
using RabbitMQ.Client;

namespace Mango.RabbitMQ.Services;

public class MessageProducer : IMessageProducer
{
    private readonly IRabbitMQSettings _settings;

    public MessageProducer(IRabbitMQSettings settings)
    {
        _settings = settings;
    }

    public void PublishMessage<T>(T message, Queue queue)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };

        using var connection = factory.CreateConnection();

        using var channel = connection.CreateModel();

        string queueName;
        switch (queue)
        {
            case Queue.Checkout:
                queueName = _settings.CheckoutMessageQueueName;
                break;
            
            case Queue.Payment:
                queueName = _settings.PaymentMessageQueueName;
                break;
            
            case Queue.PaymentStatusUpdate:
                queueName = _settings.PaymentUpdateMessageQueueName;
                break;
            
            default:
                queueName = _settings.CheckoutMessageQueueName;
                break;
        }

        channel.QueueDeclare(queueName, durable: true, exclusive: false);

        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);
        
        channel.BasicPublish("", queueName, body:body);
    }
}