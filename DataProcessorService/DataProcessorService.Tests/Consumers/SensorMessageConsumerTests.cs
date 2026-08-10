using System.Text.Json;
using DataIngestorService.Contracts.Contracts;
using DataProcessorService.DAL.Interfaces;
using DataProcessorService.Logging;
using DataProcessorService.MessageConsumers;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SensorEventEntity = DataProcessorService.DAL.Entities.SensorEvent;

namespace DataProcessorService.Tests.Consumers;

public class SensorMessageConsumerTests
{
    private readonly Mock<ISensorEventRepository> _repositoryMock = new();
    private readonly ILoggingContextAccessor _loggingContext = new LoggingContextAccessor();
    private readonly SensorMessageConsumer _sut;

    public SensorMessageConsumerTests()
    {
        _repositoryMock
            .Setup(x => x.Create(It.IsAny<List<SensorEventEntity>>()))
            .ReturnsAsync((List<SensorEventEntity> entities) => entities);

        _sut = new SensorMessageConsumer(
            _repositoryMock.Object,
            _loggingContext,
            NullLogger<SensorMessageConsumer>.Instance);
    }

    [Fact]
    public async Task Consume_MapsMessageAndPersistsViaRepository()
    {
        // Arrange
        var receivedAt = new DateTime(2026, 8, 6, 15, 30, 0, DateTimeKind.Utc);
        var payload = JsonDocument.Parse("""{"humidity":55}""").RootElement.Clone();
        var message = new SensorEventsMessage
        {
            ReceivedAt = receivedAt,
            SensorEvents =
            [
                new SensorEvent
                {
                    Type = "humidity",
                    Name = "sensor-2",
                    Payload = payload
                }
            ]
        };
        var context = CreateConsumeContext(message);

        // Act
        await _sut.Consume(context.Object);

        // Assert
        _repositoryMock.Verify(
            x => x.Create(It.Is<List<SensorEventEntity>>(events =>
                events.Count == 1
                && events[0].Type == "humidity"
                && events[0].Name == "sensor-2"
                && events[0].ReceivedAt == receivedAt
                && events[0].Payload.GetProperty("humidity").GetInt32() == 55
                && events[0].Id != Guid.Empty)),
            Times.Once);
    }

    [Fact]
    public async Task Consume_WhenRepositoryFails_Rethrows()
    {
        // Arrange
        var message = new SensorEventsMessage
        {
            ReceivedAt = DateTime.UtcNow,
            SensorEvents = []
        };
        var context = CreateConsumeContext(message);
        _repositoryMock
            .Setup(x => x.Create(It.IsAny<List<SensorEventEntity>>()))
            .ThrowsAsync(new InvalidOperationException("db unavailable"));

        // Act
        var act = async () => await _sut.Consume(context.Object);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("db unavailable");
    }

    [Fact]
    public async Task Consume_WhenCancelled_ThrowsOperationCanceledException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var message = new SensorEventsMessage
        {
            ReceivedAt = DateTime.UtcNow,
            SensorEvents =
            [
                new SensorEvent
                {
                    Type = "temperature",
                    Name = "sensor-1",
                    Payload = JsonDocument.Parse("{}").RootElement.Clone()
                }
            ]
        };
        var context = CreateConsumeContext(message, cts.Token);
        _repositoryMock
            .Setup(x => x.Create(It.IsAny<List<SensorEventEntity>>()))
            .ThrowsAsync(new OperationCanceledException(cts.Token));

        // Act
        var act = async () => await _sut.Consume(context.Object);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    private static Mock<ConsumeContext<SensorEventsMessage>> CreateConsumeContext(
        SensorEventsMessage message,
        CancellationToken cancellationToken = default)
    {
        var context = new Mock<ConsumeContext<SensorEventsMessage>>();
        context.Setup(x => x.Message).Returns(message);
        context.Setup(x => x.CancellationToken).Returns(cancellationToken);
        context.Setup(x => x.CorrelationId).Returns((Guid?)null);
        context.Setup(x => x.ConversationId).Returns((Guid?)null);
        context.Setup(x => x.MessageId).Returns(NewId.NextGuid());
        return context;
    }
}
