using DataIngestorService.Contracts.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NotificationService.Hubs;
using NotificationService.Mapping;
using NotificationService.Options;
using NotificationService.RetryPolicies;

namespace NotificationService.Consumers;

public class SensorMessageConsumer(
    IHubContext<SensorEventsHub, ISensorEventsClient> sensorEventsHubContext,
    IOptions<BroadcastOptions> broadcastOptions,
    ILogger<SensorMessageConsumer> logger) : IConsumer<SensorEventsMessage>
{
    public Task Consume(ConsumeContext<SensorEventsMessage> context)
    {
        var notification = SensorNotificationMapper.ToNotification(context.Message);
        var opts = broadcastOptions.Value;
        var delay = TimeSpan.FromMilliseconds(opts.RetryDelayMilliseconds);

        logger.LogInformation(
            "Broadcasting sensor notification with {EventCount} events received at {ReceivedAt}",
            notification.Events.Count,
            notification.ReceivedAt);

        var policy = SignalRBroadcastRetryPolicies.GetSignalRBroadcastRetryPolicy(
            opts.RetryCount,
            delay,
            logger);

        return policy.ExecuteAsync(
            _ => sensorEventsHubContext.Clients.All.SensorDataUpdated(notification),
            context.CancellationToken);
    }
}
