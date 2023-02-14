using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using PaymentProcessor;

namespace Mango.Services.PaymentAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly ILogger<AzureServiceBusConsumer> _logger;
    private readonly IMessageBus _messageBus;
    private readonly IProcessPayment _processPayment;
    
    private readonly string _serviceBusConnection;
    private readonly string _paymentMessageTopicName;
    private readonly string _paymnetSubscriptionName;

    private ServiceBusProcessor _paymentProcessor;

    public AzureServiceBusConsumer(IConfiguration configuration,
        ILogger<AzureServiceBusConsumer> logger, IMessageBus messageBus, IProcessPayment processPayment)
    {
        _logger = logger;
        _messageBus = messageBus;
        _processPayment = processPayment;

        var section = configuration.GetSection("AzureMessageBusSettings");
        _serviceBusConnection = section.GetValue<string>("AzureServiceBusConnection");
        _paymentMessageTopicName = section.GetValue<string>("PaymentMessageTopicName");
        _paymnetSubscriptionName = section.GetValue<string>("PaymentSubscriptionName");

        var client = new ServiceBusClient(_serviceBusConnection);

        _paymentProcessor = client.CreateProcessor(_paymentMessageTopicName, _paymnetSubscriptionName);
    }

    public async Task Start()
    {
        _paymentProcessor.ProcessMessageAsync += ProcessPayments;
        _paymentProcessor.ProcessErrorAsync += ErrorHandler;
        await _paymentProcessor.StartProcessingAsync();
    }
    public async Task Stop()
    {
        await _paymentProcessor.StopProcessingAsync();
        await _paymentProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        _logger.LogError($" AzureServiceBusConsumer ErrorHandler method:{Environment.NewLine}{arg.Exception}");
        return Task.CompletedTask;
    }

    private async Task ProcessPayments(ProcessMessageEventArgs args)
    {
        var message = args.Message;

        var body = Encoding.UTF8.GetString(message.Body);

        var paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage updatePaymentResultMessage = new()
        {
            Status = result ? PaymentStatus.Success : PaymentStatus.Reject,
            OrderId = paymentRequestMessage.OrderId
        };
        
        try
        {
            await _messageBus.PublishMessage(updatePaymentResultMessage, Topics.PaymentUpdateMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($" AzureServiceBusConsumer ProcessPayments method" +
                             $" - update payment message error:{Environment.NewLine}{ex}");
        }
    }
}