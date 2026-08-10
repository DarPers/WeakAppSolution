using DataProcessorService.Logging;
using FluentAssertions;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace DataProcessorService.Tests.Logging;

public class LoggingContextEnricherTests
{
    [Fact]
    public void Enrich_AddsCorrelationIdAndEventCountProperties()
    {
        // Arrange
        var accessor = new LoggingContextAccessor();
        using var scope = accessor.BeginScope("abc123");
        accessor.SetEventCount(7);

        var enricher = new LoggingContextEnricher(accessor);
        var template = new MessageTemplateParser().Parse("test");
        var logEvent = new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            exception: null,
            template,
            properties: []);

        // Act
        enricher.Enrich(logEvent, new ScalarPropertyFactory());

        // Assert
        logEvent.Properties["correlation_id"].ToString().Should().Contain("abc123");
        logEvent.Properties["event_count"].ToString().Should().Contain("7");
    }

    private sealed class ScalarPropertyFactory : ILogEventPropertyFactory
    {
        public LogEventProperty CreateProperty(string name, object? value, bool destructureObjects = false) =>
            new(name, new ScalarValue(value));
    }
}
