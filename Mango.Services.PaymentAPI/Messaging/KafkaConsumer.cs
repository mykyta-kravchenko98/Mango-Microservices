using System.Text;
using Azure.Messaging.ServiceBus;
using Confluent.Kafka;
using Mango.Kafka.Configs.Interfaces;
using Mango.Kafka.Services.Interfaces;
using Mango.MessageBus;
using Mango.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using Topics = Mango.Kafka.Topics;

namespace Mango.Services.PaymentAPI.Messaging;

public class KafkaConsumer : IHostedService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IKafkaSettings _settings;
    private readonly IProcessPayment _processPayment;

    public KafkaConsumer(ILogger<KafkaConsumer> logger,
        IKafkaProducer kafkaProducer, IKafkaSettings settings, IProcessPayment processPayment)
    {
        _kafkaProducer = kafkaProducer;
        _settings = settings;
        _processPayment = processPayment;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Thread paymentThread = new Thread(StartPaymentConsumer)
        {
            Name = "PaymentConsumer"
        };
        
        paymentThread.Start();
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    //ConsumerConfig config, Action<string> action
    private void StartPaymentConsumer()
    {
        var config = new ConsumerConfig()
        {
            GroupId = _settings.PaymentGroupId,
            BootstrapServers = _settings.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var builder = new ConsumerBuilder<Null, string>(config).Build();
        
        try
        {
            builder.Subscribe(_settings.PaymentMessageTopicName);
            var cancelToken = new CancellationTokenSource();
            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    var consumer = builder.Consume(cancelToken.Token);
                    ProcessPayments(consumer.Message.Value);
                    _logger.LogTrace($"Checkout Message Recived: {consumer.Message.Value}");
                }
            }
            catch (ConsumeException e)
            {
                _logger.LogError($"Error occured: {e.Error.Reason}");
                builder.Close();
            }
        }
        catch (OperationCanceledException)
        {
            // Ensure the consumer leaves the group cleanly and final offsets are committed.
            builder.Close();
        }
    }
    
    private async void ProcessPayments(string message)
    {
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
            await Task.Factory.StartNew(() =>
                _kafkaProducer.PublishMessage(updatePaymentResultMessage, Topics.PaymentUpdateMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError($" KafkaConsumer ProcessPayments method" +
                             $" - update payment message error:{Environment.NewLine}{ex}");
        }
    }
}