using Mango.Services.PaymentAPI.Messaging;
using Mango.Services.PaymentAPI.Messaging.Interfaces;

namespace Mango.Services.PaymentAPI.Extension;

public static class ApplicationBuilderExtensions
{
    public static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }
    public static IRabbitMqConsumer RabbitMqConsumer { get; set; }

    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        RabbitMqConsumer = app.ApplicationServices.GetService<IRabbitMqConsumer>();
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife.ApplicationStarted.Register(OnStart);
        hostApplicationLife.ApplicationStopped.Register(OnStop);

        return app;
    }

    private static void OnStart()
    {
        ServiceBusConsumer.Start();
        RabbitMqConsumer.Start();
    }

    private static void OnStop()
    {
        ServiceBusConsumer.Stop();
        RabbitMqConsumer.Stop();
    }
}