using DataIngestorService.APIs;
using DataIngestorService.Contracts.Contracts;
using DataIngestorService.MessagingService.Interfaces;
using Refit;

namespace DataIngestorService.BackgroundServices;

public class SensorDataFetcherService(
    IWeakAppApi weakAppApi, 
    IMessageProducerService messageProducerService, 
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
                logger.LogError(ex, "Sensor data iteration failed");
            }
        }
    }

    private async Task RunIterationAsync(CancellationToken cancellationToken)
    {
        var events = await FetchSensorEventsAsync(cancellationToken);

        if (events.Count == 0)
            return;

        var message = new SensorEventsMessage
        {
            SensorEvents = events,
            ReceivedAt = DateTime.UtcNow
        };

        await messageProducerService.PublishSensorData(message, cancellationToken);

        logger.LogInformation("Published {EventCount} sensor event(s)", events.Count);
    }

    private async Task<List<SensorEvent>> FetchSensorEventsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await weakAppApi.GetDataAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
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
            logger.LogWarning(ex, "HTTP error calling Weak API");
            return [];
        }
    }
}
