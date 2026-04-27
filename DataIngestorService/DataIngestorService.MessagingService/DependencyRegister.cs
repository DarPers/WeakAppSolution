using DataIngestorService.MessagingService.ConfigurationOptions;
using DataIngestorService.MessagingService.Interfaces;
using DataIngestorService.MessagingService.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataIngestorService.MessagingService;

public static class DependencyRegister
{
    public static IServiceCollection AddMessageServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddSingleton<IMessageProducerService, MessageProducerService>();
        serviceCollection.AddMassTransit(busConfigurator =>
        {
            busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                busFactoryConfigurator.Host(options.HostName, options.VirtualHostName, configure =>
                {
                    configure.Username(options.UserName);
                    configure.Password(options.Password);
                });

                busFactoryConfigurator.ConfigureEndpoints(context);
            });
        });

        return serviceCollection;
    }
}
