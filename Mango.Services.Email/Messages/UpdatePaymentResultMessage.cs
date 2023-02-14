
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Mango.Services.Email.Messages;

public class UpdatePaymentResultMessage
{
    public long OrderId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus Status { get; set; }
    public string Email { get; set; }
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