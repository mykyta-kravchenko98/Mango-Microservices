using System.Text;
using Mango.RabbitMQ.Configs.Interfaces;
using Mango.RabbitMQ.Enums;
using Mango.RabbitMQ.Services.Interfaces;
using Mango.Services.PaymentAPI.Messages;
using Mango.Services.PaymentAPI.Messaging.Interfaces;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mango.Services.PaymentAPI.Messaging;

public class RabbitMqConsumer : IRabbitMqConsumer
{
    private readonly IModel _paymentChannel;
    private readonly IConnection _connection;
    private readonly IRabbitMQSettings _settings;
    private readonly IMessageProducer _producer;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly IProcessPayment _processPayment;

    public RabbitMqConsumer(IRabbitMQSettings settings,
        IMessageProducer producer, ILogger<RabbitMqConsumer> logger, IProcessPayment processPayment)
    {
        _settings = settings;
        _producer = producer;
        _logger = logger;
        _processPayment = processPayment;

        var connectionFactory = new ConnectionFactory()
        {
            HostName = settings.HostName,
            UserName = settings.UserName,
            Password = settings.Password,
            VirtualHost = settings.VirtualHost
        };

        _connection = connectionFactory.CreateConnection();
        _paymentChannel = _connection.CreateModel();
        _paymentChannel.QueueDeclare(_settings.PaymentMessageQueueName, durable: true, exclusive: false);
    }

    public Task Start()
    {
        var paymentConsumer = new EventingBasicConsumer(_paymentChannel);
        paymentConsumer.Received += ProcessPayments;
        
        _paymentChannel.BasicConsume(_settings.PaymentMessageQueueName, false, paymentConsumer);
        
        return Task.CompletedTask;
    }
    
    public Task Stop()
    {
        _paymentChannel.Close();
        _connection.Close();
        
        return Task.CompletedTask;
    }
    
    private void ProcessPayments(object? sender, BasicDeliverEventArgs args)
    {
        var body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        var paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(message);

        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage updatePaymentResultMessage = new()
        {
            Status = result ? PaymentStatus.Success : PaymentStatus.Reject,
            OrderId = paymentRequestMessage.OrderId,
            Email = paymentRequestMessage.Email
        };
        
        try
        {
            _producer.PublishMessage(updatePaymentResultMessage, Queue.PaymentStatusUpdate);
        }
        catch (Exception ex)
        {
            _logger.LogError($" RabbitMQConsumer ProcessPayments method" +
                             $" - update payment message error:{Environment.NewLine}{ex}");
        }
    }
}