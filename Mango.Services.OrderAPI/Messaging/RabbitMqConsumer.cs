using System.Text;
using AutoMapper;
using Mango.RabbitMQ.Configs.Interfaces;
using Mango.RabbitMQ.Enums;
using Mango.RabbitMQ.Services.Interfaces;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Messaging.Interfaces;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mango.Services.OrderAPI.Messaging;

public class RabbitMqConsumer : IRabbitMqConsumer
{
    private readonly IModel _checkoutChannel;
    private readonly IModel _paymentResultUpdateChannel;
    private readonly IConnection _connection;
    private readonly IRabbitMQSettings _settings;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _producer;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly OrderRepository _orderRepository;

    public RabbitMqConsumer(IRabbitMQSettings settings, IMapper mapper,
        IMessageProducer producer, ILogger<RabbitMqConsumer> logger, OrderRepository orderRepository)
    {
        _settings = settings;
        _mapper = mapper;
        _producer = producer;
        _logger = logger;
        _orderRepository = orderRepository;

        var connectionFactory = new ConnectionFactory()
        {
            HostName = settings.HostName,
            UserName = settings.UserName,
            Password = settings.Password,
            VirtualHost = settings.VirtualHost
        };

        _connection = connectionFactory.CreateConnection();
        _checkoutChannel = _connection.CreateModel();
        _paymentResultUpdateChannel = _connection.CreateModel();
        _checkoutChannel.QueueDeclare(_settings.CheckoutMessageQueueName, durable: true, exclusive: false);
        _paymentResultUpdateChannel.QueueDeclare(_settings.PaymentUpdateMessageQueueName, durable: true, exclusive: false);
    }

    public Task Start()
    {
        var checkoutMessageConsumer = new EventingBasicConsumer(_checkoutChannel);
        checkoutMessageConsumer.Received += OnCheckoutMessageReceived;
        
        _checkoutChannel.BasicConsume(_settings.CheckoutMessageQueueName, false, checkoutMessageConsumer);
        
        var updatePaymentStatusConsumer = new EventingBasicConsumer(_paymentResultUpdateChannel);
        updatePaymentStatusConsumer.Received += OnPaymentStatusUpdateMessageReceived;
        
        _paymentResultUpdateChannel.BasicConsume(_settings.PaymentUpdateMessageQueueName, false, updatePaymentStatusConsumer);
        
        return Task.CompletedTask;
    }
    
    public Task Stop()
    {
        _checkoutChannel.Close();
        _paymentResultUpdateChannel.Close();
        _connection.Close();
        
        return Task.CompletedTask;
    }
    
    private async void OnCheckoutMessageReceived(object? sender, BasicDeliverEventArgs args)
    {
        var body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        
        CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(message);

        var orderHeader = _mapper.Map<CheckoutHeaderDto, OrderHeader>(checkoutHeaderDto);

        var orderDetail = _mapper.Map<IEnumerable<CartDetailDto>, IEnumerable<OrderDetail>>(checkoutHeaderDto.CartDetails);

        orderHeader.OrderDetails = orderDetail;
        
        await Task.Factory.StartNew(() => _orderRepository.AddOrder(orderHeader)).Unwrap();

        PaymentRequestMessage paymentRequestMessage = new()
        {
            Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
            CardNumber = orderHeader.CardNumber,
            CVV = orderHeader.CVV,
            ExpireMonthYear = orderHeader.ExpireMonthYear,
            OrderId = orderHeader.OrderHeaderId,
            OrderTotal = orderHeader.OrderTotal,
            Email = orderHeader.Email
        };
        
        try
        {
            _producer.PublishMessage(paymentRequestMessage, Queue.Payment);
        }
        catch (Exception ex)
        {
            _logger.LogError($" RabbitMQConsumer OnCheckoutMessageReceived method" +
                             $" - sending payment message error:{Environment.NewLine}{ex}");
        }
    }

    private async void OnPaymentStatusUpdateMessageReceived(object? sender, BasicDeliverEventArgs args)
    {
        var body = args.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        var updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(message);

        await Task.Factory.StartNew(() => _orderRepository.UpdateOrderStatusOfPayment(updatePaymentResultMessage.OrderId,
            updatePaymentResultMessage.Status)).Unwrap();
    }
}