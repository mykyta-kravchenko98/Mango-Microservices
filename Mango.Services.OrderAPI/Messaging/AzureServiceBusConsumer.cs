using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Newtonsoft.Json;

namespace Mango.Services.OrderAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly OrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AzureServiceBusConsumer> _logger;
    
    private readonly string _serviceBusConnection;
    private readonly string _checkoutMessageTopicName;
    private readonly string _checkoutSubscriptionName;

    private ServiceBusProcessor _checkOutProcessor;

    public AzureServiceBusConsumer(OrderRepository orderRepository, IMapper mapper,
        IConfiguration configuration, ILogger<AzureServiceBusConsumer> logger)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _logger = logger;

        var section = configuration.GetSection("AzureMessageBusSettings");
        _serviceBusConnection = section.GetValue<string>("AzureServiceBusConnection");
        _checkoutMessageTopicName = section.GetValue<string>("CheckoutMessageTopicName");
        _checkoutSubscriptionName = section.GetValue<string>("CheckoutSubscriptionName");

        var client = new ServiceBusClient(_serviceBusConnection);

        _checkOutProcessor = client.CreateProcessor(_checkoutMessageTopicName, _checkoutSubscriptionName);
    }

    public async Task Start()
    {
        _checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
        _checkOutProcessor.ProcessErrorAsync += ErrorHandler;
        await _checkOutProcessor.StartProcessingAsync();
    }
    public async Task Stop()
    {
        await _checkOutProcessor.StopProcessingAsync();
        await _checkOutProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        _logger.LogError($" AzureServiceBusConsumer ErrorHandler method:{Environment.NewLine}{arg.Exception}");
        return Task.CompletedTask;
    }

    private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;

        var body = Encoding.UTF8.GetString(message.Body);

        CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

        var orderHeader = _mapper.Map<CheckoutHeaderDto, OrderHeader>(checkoutHeaderDto);

        var orderDetail = _mapper.Map<IEnumerable<CartDetailDto>, IEnumerable<OrderDetail>>(checkoutHeaderDto.CartDetails);

        orderHeader.OrderDetails = orderDetail;

        await _orderRepository.AddOrder(orderHeader);
    }
}