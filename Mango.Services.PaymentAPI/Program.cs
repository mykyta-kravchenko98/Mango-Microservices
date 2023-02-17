using System.Text.Json.Serialization;
using Mango.Kafka.Configs;
using Mango.Kafka.Configs.Interfaces;
using Mango.Kafka.Services;
using Mango.Kafka.Services.Interfaces;
using Mango.MessageBus;
using Mango.RabbitMQ.Configs;
using Mango.RabbitMQ.Configs.Interfaces;
using Mango.RabbitMQ.Services;
using Mango.RabbitMQ.Services.Interfaces;
using Mango.Services.PaymentAPI.Extension;
using Mango.Services.PaymentAPI.Messaging;
using Mango.Services.PaymentAPI.Messaging.Interfaces;
using PaymentProcessor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IProcessPayment, ProcessPayment>();

//Azure
builder.Services.AddSingleton<IMessageBusSettings, MessageBusSettingsRepository>(); 
builder.Services.AddSingleton<IMessageBus, AzureServiceMessageBus>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

//RabbitMQ
builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
builder.Services.AddSingleton<IRabbitMQSettings, RabbitMQSettingsRepository>();
builder.Services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();

//Kafka
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddSingleton<IKafkaSettings, KafkaSettingsRepository>();
builder.Services.AddHostedService<KafkaConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseAzureServiceBusConsumer();

app.Run();