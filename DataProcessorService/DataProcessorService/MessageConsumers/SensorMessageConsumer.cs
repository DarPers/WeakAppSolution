using DataIngestorService.Contracts.Contracts;
using DataProcessorService.DAL.Interfaces;
using DataProcessorService.Logging;
using DataProcessorService.Mapping;
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

        using var scope = loggingContext.BeginScope(correlationId);

        var message = context.Message;
        var sensorEvents = SensorEventMapper.ToEntities(message);
        loggingContext.SetEventCount(sensorEvents.Count);

        logger.LogInformation(
            "Persisting {EventCount} sensor events received at {ReceivedAt}",
            sensorEvents.Count,
            message.ReceivedAt);

        try
        {
            await sensorEventRepository.Create(sensorEvents);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
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
