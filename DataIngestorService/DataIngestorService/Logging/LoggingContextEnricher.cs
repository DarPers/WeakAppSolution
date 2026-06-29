using Serilog.Core;
using Serilog.Events;

namespace DataIngestorService.Logging;

public sealed class LoggingContextEnricher(ILoggingContextAccessor contextAccessor) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("correlation_id", contextAccessor.CorrelationId));

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("event_count", contextAccessor.EventCount));
    }
}
