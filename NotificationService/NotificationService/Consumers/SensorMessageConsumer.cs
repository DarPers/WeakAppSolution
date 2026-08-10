using DataIngestorService.Contracts.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NotificationService.Hubs;
using NotificationService.Mapping;
using NotificationService.Options;
using NotificationService.RetryPolicies;
using NotificationService.Telemetry;

namespace NotificationService.Consumers;

public class SensorMessageConsumer(
    IHubContext<SensorEventsHub, ISensorEventsClient> sensorEventsHubContext,
    IOptions<BroadcastOptions> broadcastOptions,
    ILogger<SensorMessageConsumer> logger) : IConsumer<SensorEventsMessage>
{
    public async Task Consume(ConsumeContext<SensorEventsMessage> context)
    {
        var notification = SensorNotificationMapper.ToNotification(context.Message);
        var opts = broadcastOptions.Value;
        var delay = TimeSpan.FromMilliseconds(opts.RetryDelayMilliseconds);

        using var activity = ServiceTelemetry.ActivitySource.StartActivity("sensor.broadcast");
        activity?.SetTag("sensor.event_count", notification.Events.Count);

        logger.LogInformation(
            "Broadcasting sensor notification with {EventCount} events received at {ReceivedAt}",
            notification.Events.Count,
            notification.ReceivedAt);

        var policy = SignalRBroadcastRetryPolicies.GetSignalRBroadcastRetryPolicy(
            opts.RetryCount,
            delay,
            logger);

        try
        {
            await policy.ExecuteAsync(
                _ => sensorEventsHubContext.Clients.All.SensorDataUpdated(notification),
                context.CancellationToken);

            ServiceTelemetry.EventsBroadcast.Add(notification.Events.Count);
        }
        catch
        {
            ServiceTelemetry.BroadcastErrors.Add(1);
            throw;
        }
    }
}
