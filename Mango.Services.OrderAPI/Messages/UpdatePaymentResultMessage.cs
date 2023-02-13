using Mango.Services.OrderAPI.Models;

namespace Mango.Services.OrderAPI.Messages;

public class UpdatePaymentResultMessage
{
    public long OrderId { get; set; }
    public PaymentStatus Status { get; set; }
}