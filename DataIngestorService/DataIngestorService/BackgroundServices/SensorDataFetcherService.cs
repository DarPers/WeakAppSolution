using DataIngestorService.APIs;
using DataIngestorService.Contracts.Contracts;
using DataIngestorService.Logging;
using DataIngestorService.MessagingService.Interfaces;
using DataIngestorService.Telemetry;
using Refit;

namespace DataIngestorService.BackgroundServices;

public class SensorDataFetcherService(
    IWeakAppApi weakAppApi,
    IMessageProducerService messageProducerService,
    ILoggingContextAccessor loggingContext,
    ILogger<SensorDataFetcherService> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(15);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(PollInterval);

        while (await timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                await RunIterationAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                ServiceTelemetry.FetchErrors.Add(1);
                logger.LogError(ex, "Sensor data iteration failed");
            }
        }
    }

    private async Task RunIterationAsync(CancellationToken cancellationToken)
    {
        using var activity = ServiceTelemetry.ActivitySource.StartActivity("sensor.fetch_and_publish");
        using var scope = loggingContext.BeginScope();

        var events = await FetchSensorEventsAsync(cancellationToken);
        loggingContext.SetEventCount(events.Count);
        activity?.SetTag("sensor.event_count", events.Count);

        if (events.Count == 0)
            return;

        ServiceTelemetry.EventsFetched.Add(events.Count);

        var message = new SensorEventsMessage
        {
            SensorEvents = events,
            ReceivedAt = DateTime.UtcNow
        };

        await messageProducerService.PublishSensorData(message, cancellationToken);
        ServiceTelemetry.EventsPublished.Add(events.Count);

        logger.LogInformation("Published sensor events");
    }

    private async Task<List<SensorEvent>> FetchSensorEventsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await weakAppApi.GetDataAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                ServiceTelemetry.FetchErrors.Add(1);
                logger.LogWarning(
                    "Weak API returned {StatusCode} {ReasonPhrase}",
                    (int)response.StatusCode,
                    response.ReasonPhrase);
                return [];
            }
            return response.Content ?? [];
        }
        catch (ApiException ex)
        {
            ServiceTelemetry.FetchErrors.Add(1);
            logger.LogWarning(ex, "HTTP error calling Weak API");
            return [];
        }
    }
}
