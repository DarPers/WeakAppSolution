namespace DataProcessorService.Logging;

public interface ILoggingContextAccessor
{
    string CorrelationId { get; }

    int EventCount { get; }

    IDisposable BeginScope(string? correlationId = null);

    void SetEventCount(int eventCount);
}
