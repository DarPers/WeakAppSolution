namespace DataIngestorService.Logging;

public interface ILoggingContextAccessor
{
    public string CorrelationId { get; }

    public int EventCount { get; }

    public IDisposable BeginScope(string? correlationId = null);

    public void SetEventCount(int eventCount);
}
