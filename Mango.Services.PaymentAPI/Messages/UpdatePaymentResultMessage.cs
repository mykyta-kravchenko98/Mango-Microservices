using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Mango.MessageBus;

namespace Mango.Services.PaymentAPI.Messages;

public class UpdatePaymentResultMessage : BaseMessage
{
    public long OrderId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus Status { get; set; }
}

public enum PaymentStatus
{
    [EnumMember(Value = "New")]
    New,
    [EnumMember(Value = "Success")]
    Success,
    [EnumMember(Value = "Reject")]
    Reject
}