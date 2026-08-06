using System.Text.Json;
using DataIngestorService.Contracts.Contracts;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NotificationService.Consumers;
using NotificationService.Dtos;
using NotificationService.Hubs;
using NotificationService.Options;

namespace NotificationService.Tests.Consumers;

public class SensorMessageConsumerTests
{
    private readonly Mock<IHubContext<SensorEventsHub, ISensorEventsClient>> _hubContextMock = new();
    private readonly Mock<IHubClients<ISensorEventsClient>> _hubClientsMock = new();
    private readonly Mock<ISensorEventsClient> _clientProxyMock = new();
    private readonly SensorMessageConsumer _sut;

    public SensorMessageConsumerTests()
    {
        _hubContextMock.Setup(x => x.Clients).Returns(_hubClientsMock.Object);
        _hubClientsMock.Setup(x => x.All).Returns(_clientProxyMock.Object);
        _clientProxyMock
            .Setup(x => x.SensorDataUpdated(It.IsAny<SensorNotificationDto>()))
            .Returns(Task.CompletedTask);

        _sut = CreateSut(retryCount: 0);
    }

    [Fact]
    public async Task Consume_MapsMessageAndBroadcastsTypedNotification()
    {
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

        await _sut.Consume(context.Object);

        _clientProxyMock.Verify(
            x => x.SensorDataUpdated(It.Is<SensorNotificationDto>(n =>
                n.ReceivedAt == receivedAt
                && n.Events.Count == 1
                && n.Events[0].Type == "humidity"
                && n.Events[0].Name == "sensor-2"
                && n.Events[0].Payload.GetProperty("humidity").GetInt32() == 55)),
            Times.Once);
    }

    [Fact]
    public async Task Consume_WhenBroadcastFails_Rethrows()
    {
        var message = new SensorEventsMessage
        {
            ReceivedAt = DateTime.UtcNow,
            SensorEvents = []
        };
        var context = CreateConsumeContext(message);
        _clientProxyMock
            .Setup(x => x.SensorDataUpdated(It.IsAny<SensorNotificationDto>()))
            .ThrowsAsync(new HubException("broadcast failed"));

        var act = async () => await _sut.Consume(context.Object);

        await act.Should().ThrowAsync<HubException>().WithMessage("broadcast failed");
    }

    [Fact]
    public async Task Consume_WhenBroadcastFails_RetriesConfiguredTimesThenRethrows()
    {
        var sut = CreateSut(retryCount: 2);
        var message = new SensorEventsMessage
        {
            ReceivedAt = DateTime.UtcNow,
            SensorEvents = []
        };
        var context = CreateConsumeContext(message);
        _clientProxyMock
            .Setup(x => x.SensorDataUpdated(It.IsAny<SensorNotificationDto>()))
            .ThrowsAsync(new HubException("broadcast failed"));

        var act = async () => await sut.Consume(context.Object);

        await act.Should().ThrowAsync<HubException>();
        _clientProxyMock.Verify(
            x => x.SensorDataUpdated(It.IsAny<SensorNotificationDto>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task Consume_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var message = new SensorEventsMessage
        {
            ReceivedAt = DateTime.UtcNow,
            SensorEvents = []
        };
        var context = CreateConsumeContext(message, cts.Token);

        var act = async () => await _sut.Consume(context.Object);

        await act.Should().ThrowAsync<OperationCanceledException>();
        _clientProxyMock.Verify(
            x => x.SensorDataUpdated(It.IsAny<SensorNotificationDto>()),
            Times.Never);
    }

    private SensorMessageConsumer CreateSut(int retryCount) =>
        new(
            _hubContextMock.Object,
            Microsoft.Extensions.Options.Options.Create(new BroadcastOptions
            {
                RetryCount = retryCount,
                RetryDelayMilliseconds = 0
            }),
            NullLogger<SensorMessageConsumer>.Instance);

    private static Mock<ConsumeContext<SensorEventsMessage>> CreateConsumeContext(
        SensorEventsMessage message,
        CancellationToken cancellationToken = default)
    {
        var context = new Mock<ConsumeContext<SensorEventsMessage>>();
        context.Setup(x => x.Message).Returns(message);
        context.Setup(x => x.CancellationToken).Returns(cancellationToken);
        return context;
    }
}
