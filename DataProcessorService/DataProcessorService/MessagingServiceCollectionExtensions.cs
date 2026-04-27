using DataProcessorService.ConfigurationOptions;
using DataProcessorService.MessageConsumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataProcessorService;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddMassTransit(config =>
        {
            config.AddConsumers(typeof(Program).Assembly);

            config.UsingRabbitMq((context, connectionSettings) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                connectionSettings.Host(options.HostName, options.VirtualHostName, h =>
                {
                    h.Username(options.UserName);
                    h.Password(options.Password);
                });

                connectionSettings.ReceiveEndpoint(options.QueueName, ep =>
                {
                    ep.ConfigureConsumer<SensorMessageConsumer>(context);
                });
            });
        });

        return serviceCollection;
    }
}
