using DataProcessorService.Logging;
using FluentAssertions;

namespace DataProcessorService.Tests.Logging;

public class LoggingContextAccessorTests
{
    private readonly LoggingContextAccessor _sut = new();

    [Fact]
    public void BeginScope_SetsCorrelationIdAndResetsEventCount()
    {
        // Act
        using var scope = _sut.BeginScope("corr-123");
        _sut.SetEventCount(5);

        // Assert
        _sut.CorrelationId.Should().Be("corr-123");
        _sut.EventCount.Should().Be(5);
    }

    [Fact]
    public void BeginScope_WhenCorrelationIdOmitted_GeneratesNonEmptyId()
    {
        // Act
        using var scope = _sut.BeginScope();

        // Assert
        _sut.CorrelationId.Should().NotBeNullOrWhiteSpace();
        _sut.EventCount.Should().Be(0);
    }

    [Fact]
    public void Dispose_ClearsCorrelationIdAndEventCount()
    {
        // Arrange
        var scope = _sut.BeginScope("corr-xyz");
        _sut.SetEventCount(3);

        // Act
        scope.Dispose();

        // Assert
        _sut.CorrelationId.Should().BeEmpty();
        _sut.EventCount.Should().Be(0);
    }
}
