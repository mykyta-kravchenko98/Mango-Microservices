using Mango.RabbitMQ.Enums;

namespace Mango.RabbitMQ.Services.Interfaces;

public interface IMessageProducer
{
    void PublishMessage<T>(T message, Queue queue);
}