namespace Mango.Kafka.Configs.Interfaces;

public interface IKafkaSettings
{
    string CheckoutMessageTopicName { get; }
    string PaymentMessageTopicName { get; }
    string PaymentUpdateMessageTopicName { get; }
    string BootstrapServers { get; }
    string CheckoutGroupId { get; }
    string PaymentGroupId { get; }
    string PaymentStatusUpdateGroupId { get; }
}