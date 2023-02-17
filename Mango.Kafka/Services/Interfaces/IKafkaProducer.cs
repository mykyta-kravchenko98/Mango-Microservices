namespace Mango.Kafka.Services.Interfaces;

public interface IKafkaProducer
{
    Task PublishMessage<T>(T message, Topics topic);
}