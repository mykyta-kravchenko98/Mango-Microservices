using Confluent.Kafka;
using Mango.Kafka.Configs.Interfaces;
using Mango.Kafka.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mango.Kafka.Services;

public class KafkaProducer : IKafkaProducer
{
    private readonly IKafkaSettings _settings;

    public KafkaProducer(IKafkaSettings settings)
    {
        _settings = settings;
    }

    public async Task PublishMessage<T>(T message, Topics topic)
    {
        string topicName;
        switch (topic)
        {
            case Topics.CheckoutMessage:
                topicName = _settings.CheckoutMessageTopicName;
                break;

            case Topics.PaymentMessage:
                topicName = _settings.PaymentMessageTopicName;
                break;

            case Topics.PaymentUpdateMessage:
                topicName = _settings.PaymentUpdateMessageTopicName;
                break;

            default:
                topicName = _settings.CheckoutMessageTopicName;
                break;
        }

        var config = new ProducerConfig()
        {
            BootstrapServers = _settings.BootstrapServers
        };

        using var producer = new ProducerBuilder<Null, string>(config).Build();

        var stringEnumConverter = new StringEnumConverter();
        var jsonMessage = JsonConvert.SerializeObject(message, stringEnumConverter);
        var kafkaMessage = new Message<Null, string>()
        {
            Value = jsonMessage
        };

        var response = await producer.ProduceAsync(topicName, kafkaMessage);
    }
}