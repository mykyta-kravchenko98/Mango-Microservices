using AutoMapper;
using Confluent.Kafka;
using Mango.Kafka.Configs.Interfaces;
using Mango.Kafka.Services.Interfaces;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Newtonsoft.Json;
using Topics = Mango.Kafka.Topics;

namespace Mango.Services.OrderAPI.Messaging;

public class KafkaConsumer : IHostedService
{
    private readonly OrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IKafkaSettings _settings;

    public KafkaConsumer(OrderRepository orderRepository, IMapper mapper,
        ILogger<KafkaConsumer> logger, IKafkaProducer kafkaProducer, IKafkaSettings settings)
    {
        _kafkaProducer = kafkaProducer;
        _settings = settings;
        _orderRepository = orderRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Thread checkoutThread = new Thread(StartCheckoutConsumer)
        {
            Name = "CheckoutConsumer"
        };
        
        Thread paymentStatusUpdateThread = new Thread(StartPaymentStatusUpdateConsumer)
        {
            Name = "PaymentStatusUpdateConsumer"
        };
        
        checkoutThread.Start();
        paymentStatusUpdateThread.Start();
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    private void StartCheckoutConsumer()
    {
        var config = new ConsumerConfig()
        {
            GroupId = _settings.CheckoutGroupId,
            BootstrapServers = _settings.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var builder = new ConsumerBuilder<Null, string>(config).Build();
        
        try
        {
            builder.Subscribe(_settings.CheckoutMessageTopicName);
            var cancelToken = new CancellationTokenSource();
            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    var consumer = builder.Consume(cancelToken.Token);
                    OnCheckoutMessageReceived(consumer.Message.Value);
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
    
    private void StartPaymentStatusUpdateConsumer()
    {
        var config = new ConsumerConfig()
        {
            GroupId = _settings.PaymentStatusUpdateGroupId,
            BootstrapServers = _settings.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var builder = new ConsumerBuilder<Null, string>(config).Build();
        
        try
        {
            builder.Subscribe(_settings.PaymentUpdateMessageTopicName);
            var cancelToken = new CancellationTokenSource();
            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    var consumer = builder.Consume(cancelToken.Token);
                    OnPaymentStatusUpdateMessageReceived(consumer.Message.Value);
                    _logger.LogTrace($"Payment Status Update Message Recived: {consumer.Message.Value}");
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
    
    private async void OnCheckoutMessageReceived(string message)
    {
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
            await Task.Factory.StartNew(() =>
                _kafkaProducer.PublishMessage(paymentRequestMessage, Topics.PaymentMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError($" KafkaConsumer OnCheckoutMessageReceived method" +
                             $" - sending payment message error:{Environment.NewLine}{ex}");
        }
    }
    
    private async void OnPaymentStatusUpdateMessageReceived(string message)
    {
        var updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(message);

        await Task.Factory.StartNew(() => _orderRepository.UpdateOrderStatusOfPayment(updatePaymentResultMessage.OrderId,
            updatePaymentResultMessage.Status));
    }
    
}