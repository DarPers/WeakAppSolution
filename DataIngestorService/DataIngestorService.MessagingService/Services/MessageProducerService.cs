using DataIngestorService.Contracts.Contracts;
using DataIngestorService.MessagingService.ConfigurationOptions;
using DataIngestorService.MessagingService.Interfaces;
using DataIngestorService.MessagingService.RetryPolicies;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataIngestorService.MessagingService.Services;

public class MessageProducerService(
    IBus bus,
    IOptions<RabbitMqOptions> options,
    ILogger<MessageProducerService> logger) : IMessageProducerService
{
    public Task PublishSensorData(SensorEventsMessage sensorEventMessage, CancellationToken cancellationToken)
    {
        var opts = options.Value;
        var delay = TimeSpan.FromMilliseconds(opts.PublishRetryDelayMilliseconds);

        var policy = RabbitMqPublishRetryPolicies.GetRabbitMqPublishRetryPolicy(
            opts.PublishRetryCount,
            delay,
            logger);

        return policy.ExecuteAsync(
            ct => bus.Publish(sensorEventMessage, ct),
            cancellationToken);
    }
}
