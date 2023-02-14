using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;
using Newtonsoft.Json;

namespace Mango.Services.Email.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly EmailRepository _emailRepository;
    private readonly ILogger<AzureServiceBusConsumer> _logger;

    private readonly ServiceBusProcessor _emailProcessor;

    public AzureServiceBusConsumer(EmailRepository emailRepository, IConfiguration configuration,
        ILogger<AzureServiceBusConsumer> logger)
    {
        _emailRepository = emailRepository;
        _logger = logger;

        var section = configuration.GetSection("AzureMessageBusSettings");
        var serviceBusConnection = section.GetValue<string>("AzureServiceBusConnection");
        var emailSubscriptionName = section.GetValue<string>("EmailSubscriptionName");
        var updatePaymentResultTopicName = section.GetValue<string>("UpdatePaymentResultTopicName");

        var client = new ServiceBusClient(serviceBusConnection);

        _emailProcessor = client.CreateProcessor(updatePaymentResultTopicName, emailSubscriptionName);
    }

    public async Task Start()
    {
        _emailProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        _emailProcessor.ProcessErrorAsync += ErrorHandler;
        await _emailProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await _emailProcessor.StopProcessingAsync();
        await _emailProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        _logger.LogError($" AzureServiceBusConsumer ErrorHandler method:{Environment.NewLine}{arg.Exception}");
        return Task.CompletedTask;
    }

    private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;

        var body = Encoding.UTF8.GetString(message.Body);

        var updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

        try
        {
            await _emailRepository.SendAndLogEmail(updatePaymentResultMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(OnOrderPaymentUpdateReceived)} SendAndLogEmail error:{Environment.NewLine}{ex}");
        }
    }
}