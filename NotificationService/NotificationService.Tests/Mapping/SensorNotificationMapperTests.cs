using System.Text.Json;
using DataIngestorService.Contracts.Contracts;
using FluentAssertions;
using NotificationService.Dtos;
using NotificationService.Mapping;

namespace NotificationService.Tests.Mapping;

public class SensorNotificationMapperTests
{
    [Fact]
    public void ToNotification_MapsMessageFieldsToDto()
    {
        var receivedAt = new DateTime(2026, 8, 6, 12, 0, 0, DateTimeKind.Utc);
        var payload = JsonDocument.Parse("""{"value":42}""").RootElement.Clone();
        var message = new SensorEventsMessage
        {
            ReceivedAt = receivedAt,
            SensorEvents =
            [
                new SensorEvent
                {
                    Type = "temperature",
                    Name = "sensor-1",
                    Payload = payload
                }
            ]
        };

        var notification = SensorNotificationMapper.ToNotification(message);

        notification.ReceivedAt.Should().Be(receivedAt);
        notification.Events.Should().HaveCount(1);
        notification.Events[0].Type.Should().Be("temperature");
        notification.Events[0].Name.Should().Be("sensor-1");
        notification.Events[0].Payload.GetProperty("value").GetInt32().Should().Be(42);
    }

    [Fact]
    public void ToNotification_WhenMessageIsNull_ThrowsArgumentNullException()
    {
        var act = () => SensorNotificationMapper.ToNotification(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToNotification_WhenSensorEventsIsNull_ThrowsArgumentNullException()
    {
        var message = new SensorEventsMessage
        {
            ReceivedAt = DateTime.UtcNow,
            SensorEvents = null!
        };

        var act = () => SensorNotificationMapper.ToNotification(message);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToNotification_WhenSensorEventsEmpty_ReturnsEmptyEvents()
    {
        var message = new SensorEventsMessage
        {
            ReceivedAt = DateTime.UtcNow,
            SensorEvents = []
        };

        var notification = SensorNotificationMapper.ToNotification(message);

        notification.Events.Should().BeEmpty();
    }
}
