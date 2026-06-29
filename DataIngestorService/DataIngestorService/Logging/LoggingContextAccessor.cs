namespace DataIngestorService.Logging;

public sealed class LoggingContextAccessor : ILoggingContextAccessor
{
    private static readonly AsyncLocal<string?> CorrelationIdLocal = new();
    private static readonly AsyncLocal<int> EventCountLocal = new();

    public string CorrelationId => CorrelationIdLocal.Value ?? string.Empty;

    public int EventCount => EventCountLocal.Value;

    public IDisposable BeginScope(string? correlationId = null)
    {
        CorrelationIdLocal.Value = correlationId ?? Guid.NewGuid().ToString("N");
        EventCountLocal.Value = 0;
        return new Scope();
    }

    public void SetEventCount(int eventCount) => EventCountLocal.Value = eventCount;

    private sealed class Scope : IDisposable
    {
        public void Dispose()
        {
            CorrelationIdLocal.Value = null;
            EventCountLocal.Value = 0;
        }
    }
}
