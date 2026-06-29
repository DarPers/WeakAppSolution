using System.Net;
using System.Reflection;
using System.Text.Json;
using DataIngestorService.APIs;
using DataIngestorService.BackgroundServices;
using DataIngestorService.Contracts.Contracts;
using DataIngestorService.Logging;
using DataIngestorService.MessagingService.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Refit;

namespace DataIngestorService.Tests.BackgroundServices;

public class SensorDataFetcherServiceTests
{
    private readonly Mock<IWeakAppApi> _weakAppApiMock = new();
    private readonly Mock<IMessageProducerService> _messageProducerMock = new();
    private readonly Mock<ILoggingContextAccessor> _loggingContextMock = new();
    private readonly Mock<ILogger<SensorDataFetcherService>> _loggerMock = new();
    private readonly SensorDataFetcherService _sut;

    public SensorDataFetcherServiceTests()
    {
        _loggingContextMock
            .Setup(x => x.BeginScope(It.IsAny<string?>()))
            .Returns(Mock.Of<IDisposable>());

        _sut = new SensorDataFetcherService(
            _weakAppApiMock.Object,
            _messageProducerMock.Object,
            _loggingContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task FetchSensorEventsAsync_WhenApiReturns200_ReturnsSensorEvents()
    {
        // Arrange
        var expectedEvents = new List<SensorEvent> { CreateSensorEvent() };
        _weakAppApiMock
            .Setup(x => x.GetDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateApiResponse(HttpStatusCode.OK, expectedEvents));

        // Act
        var result = await InvokeFetchSensorEventsAsync(_sut, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedEvents);
        _weakAppApiMock.Verify(x => x.GetDataAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FetchSensorEventsAsync_WhenApiReturns500_ReturnsEmptyList()
    {
        // Arrange
        _weakAppApiMock
            .Setup(x => x.GetDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateApiResponse(HttpStatusCode.InternalServerError, content: null));

        // Act
        var result = await InvokeFetchSensorEventsAsync(_sut, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _messageProducerMock.Verify(
            x => x.PublishSensorData(It.IsAny<SensorEventsMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task FetchSensorEventsAsync_WhenApiReturns429_ReturnsEmptyList()
    {
        // Arrange
        _weakAppApiMock
            .Setup(x => x.GetDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateApiResponse(HttpStatusCode.TooManyRequests, content: null));

        // Act
        var result = await InvokeFetchSensorEventsAsync(_sut, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _messageProducerMock.Verify(
            x => x.PublishSensorData(It.IsAny<SensorEventsMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RunIterationAsync_WhenApiReturns200_PublishesSensorEventsMessage()
    {
        // Arrange
        var expectedEvents = new List<SensorEvent> { CreateSensorEvent() };
        _weakAppApiMock
            .Setup(x => x.GetDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateApiResponse(HttpStatusCode.OK, expectedEvents));

        // Act
        await InvokeRunIterationAsync(_sut, CancellationToken.None);

        // Assert
        _loggingContextMock.Verify(x => x.SetEventCount(expectedEvents.Count), Times.Once);
        _messageProducerMock.Verify(
            x => x.PublishSensorData(
                It.Is<SensorEventsMessage>(message =>
                    message.SensorEvents == expectedEvents
                    && message.ReceivedAt != default),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RunIterationAsync_WhenApiReturns500_DoesNotPublishMessage()
    {
        // Arrange
        _weakAppApiMock
            .Setup(x => x.GetDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateApiResponse(HttpStatusCode.InternalServerError, content: null));

        // Act
        await InvokeRunIterationAsync(_sut, CancellationToken.None);

        // Assert
        _loggingContextMock.Verify(x => x.SetEventCount(0), Times.Once);
        _messageProducerMock.Verify(
            x => x.PublishSensorData(It.IsAny<SensorEventsMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RunIterationAsync_WhenApiReturns429_DoesNotPublishMessage()
    {
        // Arrange
        _weakAppApiMock
            .Setup(x => x.GetDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateApiResponse(HttpStatusCode.TooManyRequests, content: null));

        // Act
        await InvokeRunIterationAsync(_sut, CancellationToken.None);

        // Assert
        _loggingContextMock.Verify(x => x.SetEventCount(0), Times.Once);
        _messageProducerMock.Verify(
            x => x.PublishSensorData(It.IsAny<SensorEventsMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static SensorEvent CreateSensorEvent()
    {
        using var document = JsonDocument.Parse("""{"value":42}""");
        return new SensorEvent
        {
            Type = "temperature",
            Name = "sensor-1",
            Payload = document.RootElement.Clone()
        };
    }

    private static ApiResponse<List<SensorEvent>> CreateApiResponse(
        HttpStatusCode statusCode,
        List<SensorEvent>? content)
    {
        return new ApiResponse<List<SensorEvent>>(
            new HttpResponseMessage(statusCode),
            content,
            new RefitSettings());
    }

    private static Task<List<SensorEvent>> InvokeFetchSensorEventsAsync(
        SensorDataFetcherService service,
        CancellationToken cancellationToken)
    {
        var method = typeof(SensorDataFetcherService).GetMethod(
            "FetchSensorEventsAsync",
            BindingFlags.Instance | BindingFlags.NonPublic);

        return (Task<List<SensorEvent>>)method!.Invoke(service, [cancellationToken])!;
    }

    private static Task InvokeRunIterationAsync(
        SensorDataFetcherService service,
        CancellationToken cancellationToken)
    {
        var method = typeof(SensorDataFetcherService).GetMethod(
            "RunIterationAsync",
            BindingFlags.Instance | BindingFlags.NonPublic);

        return (Task)method!.Invoke(service, [cancellationToken])!;
    }
}
