using DataIngestorService.Contracts.Contracts;
using DataProcessorService.DAL.Interfaces;
using DataProcessorService.Logging;
using DataProcessorService.Mapping;
using DataProcessorService.Telemetry;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DataProcessorService.MessageConsumers;

public class SensorMessageConsumer(
    ISensorEventRepository sensorEventRepository,
    ILoggingContextAccessor loggingContext,
    ILogger<SensorMessageConsumer> logger) : IConsumer<SensorEventsMessage>
{
    public async Task Consume(ConsumeContext<SensorEventsMessage> context)
    {
        var correlationId = context.CorrelationId?.ToString("N")
            ?? context.ConversationId?.ToString("N")
            ?? context.MessageId?.ToString("N");

        using var activity = ServiceTelemetry.ActivitySource.StartActivity("sensor.persist");
        using var scope = loggingContext.BeginScope(correlationId);

        var message = context.Message;
        var sensorEvents = SensorEventMapper.ToEntities(message);
        loggingContext.SetEventCount(sensorEvents.Count);
        activity?.SetTag("sensor.event_count", sensorEvents.Count);
        activity?.SetTag("messaging.correlation_id", correlationId);

        logger.LogInformation(
            "Persisting {EventCount} sensor events received at {ReceivedAt}",
            sensorEvents.Count,
            message.ReceivedAt);

        try
        {
            await sensorEventRepository.Create(sensorEvents);
            ServiceTelemetry.EventsPersisted.Add(sensorEvents.Count);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ServiceTelemetry.PersistErrors.Add(1);
            // Rethrow so MassTransit retries and eventually moves the message to the error queue.
            logger.LogError(
                ex,
                "Failed to persist {EventCount} sensor events; message will be retried or moved to the error queue",
                sensorEvents.Count);
            throw;
        }

        logger.LogInformation("Persisted sensor events");
    }
}
