using DataProcessorService.Logging;
using FluentAssertions;

namespace DataProcessorService.Tests.Logging;

public class LoggingContextAccessorTests
{
    private readonly LoggingContextAccessor _sut = new();

    [Fact]
    public void BeginScope_SetsCorrelationIdAndResetsEventCount()
    {
        using var scope = _sut.BeginScope("corr-123");
        _sut.SetEventCount(5);

        _sut.CorrelationId.Should().Be("corr-123");
        _sut.EventCount.Should().Be(5);
    }

    [Fact]
    public void BeginScope_WhenCorrelationIdOmitted_GeneratesNonEmptyId()
    {
        using var scope = _sut.BeginScope();

        _sut.CorrelationId.Should().NotBeNullOrWhiteSpace();
        _sut.EventCount.Should().Be(0);
    }

    [Fact]
    public void Dispose_ClearsCorrelationIdAndEventCount()
    {
        var scope = _sut.BeginScope("corr-xyz");
        _sut.SetEventCount(3);

        scope.Dispose();

        _sut.CorrelationId.Should().BeEmpty();
        _sut.EventCount.Should().Be(0);
    }
}
