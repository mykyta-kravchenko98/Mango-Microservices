using Mango.MessageBus;

namespace Mango.Services.PaymentAPI.Messages;

public class PaymentRequestMessage : BaseMessage
{
    public long OrderId { get; set; }
    public string Name { get; set; }
    public string CardNumber { get; set; }
    public string CVV { get; set; }
    public string ExpireMonthYear { get; set; }
    public double OrderTotal { get; set; }
}