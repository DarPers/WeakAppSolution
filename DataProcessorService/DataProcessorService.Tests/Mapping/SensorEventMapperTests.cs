using System.Text.Json;
using DataIngestorService.Contracts.Contracts;
using DataProcessorService.Mapping;
using FluentAssertions;

namespace DataProcessorService.Tests.Mapping;

public class SensorEventMapperTests
{
    [Fact]
    public void ToEntities_MapsMessageFieldsToEntities()
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

        var entities = SensorEventMapper.ToEntities(message);

        entities.Should().HaveCount(1);
        entities[0].Id.Should().NotBe(Guid.Empty);
        entities[0].Type.Should().Be("temperature");
        entities[0].Name.Should().Be("sensor-1");
        entities[0].ReceivedAt.Should().Be(receivedAt);
        entities[0].Payload.GetProperty("value").GetInt32().Should().Be(42);
    }

    [Fact]
    public void ToEntities_WhenMessageIsNull_ThrowsArgumentNullException()
    {
        var act = () => SensorEventMapper.ToEntities(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToEntities_WhenSensorEventsIsNull_ThrowsArgumentNullException()
    {
        var message = new SensorEventsMessage
        {
            ReceivedAt = DateTime.UtcNow,
            SensorEvents = null!
        };

        var act = () => SensorEventMapper.ToEntities(message);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToEntities_WhenSensorEventsEmpty_ReturnsEmptyList()
    {
        var message = new SensorEventsMessage
        {
            ReceivedAt = DateTime.UtcNow,
            SensorEvents = []
        };

        var entities = SensorEventMapper.ToEntities(message);

        entities.Should().BeEmpty();
    }

    [Fact]
    public void ToEntities_AssignsDistinctIdsPerEvent()
    {
        var payload = JsonDocument.Parse("{}").RootElement.Clone();
        var message = new SensorEventsMessage
        {
            ReceivedAt = DateTime.UtcNow,
            SensorEvents =
            [
                new SensorEvent { Type = "a", Name = "n1", Payload = payload },
                new SensorEvent { Type = "b", Name = "n2", Payload = payload }
            ]
        };

        var entities = SensorEventMapper.ToEntities(message);

        entities.Select(e => e.Id).Should().OnlyHaveUniqueItems();
    }
}
