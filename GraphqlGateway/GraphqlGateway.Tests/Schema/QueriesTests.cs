using System.Text.Json;
using FluentAssertions;
using GraphqlGateway.Data;
using GraphqlGateway.Models;
using GraphqlGateway.Schema;
using Microsoft.EntityFrameworkCore;

namespace GraphqlGateway.Tests.Schema;

public class QueriesTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Queries _queries = new();

    public QueriesTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public void GetSensorEvents_ReturnsAllSeededEvents()
    {
        // Arrange
        Seed(
            Event("temperature", "sensor-a"),
            Event("humidity", "sensor-b"));

        // Act
        var events = _queries.GetSensorEvents(_dbContext).ToList();

        // Assert
        events.Should().HaveCount(2);
        events.Select(e => e.Type).Should().BeEquivalentTo("temperature", "humidity");
    }

    [Fact]
    public void GetSensorEvents_WhenEmpty_ReturnsEmptyQueryable()
    {
        // Act
        var events = _queries.GetSensorEvents(_dbContext).ToList();

        // Assert
        events.Should().BeEmpty();
    }

    [Fact]
    public void GetStatsByType_GroupsAndCountsByType()
    {
        // Arrange
        Seed(
            Event("temperature", "sensor-a"),
            Event("temperature", "sensor-b"),
            Event("humidity", "sensor-c"));

        // Act
        var stats = _queries.GetStatsByType(_dbContext).ToList();

        // Assert
        stats.Should().BeEquivalentTo(
        [
            new Queries.TypeStats("temperature", 2),
            new Queries.TypeStats("humidity", 1)
        ]);
    }

    [Fact]
    public void GetStatsByType_WhenEmpty_ReturnsEmpty()
    {
        // Act
        var stats = _queries.GetStatsByType(_dbContext).ToList();

        // Assert
        stats.Should().BeEmpty();
    }

    [Fact]
    public void GetStatsByLocation_GroupsAndCountsByName()
    {
        // Arrange
        Seed(
            Event("temperature", "warehouse"),
            Event("humidity", "warehouse"),
            Event("temperature", "office"));

        // Act
        var stats = _queries.GetStatsByLocation(_dbContext).ToList();

        // Assert
        stats.Should().BeEquivalentTo(
        [
            new Queries.TypeStats("warehouse", 2),
            new Queries.TypeStats("office", 1)
        ]);
    }

    [Fact]
    public void GetStatsByLocation_WhenEmpty_ReturnsEmpty()
    {
        // Act
        var stats = _queries.GetStatsByLocation(_dbContext).ToList();

        // Assert
        stats.Should().BeEmpty();
    }

    private void Seed(params SensorEvent[] events)
    {
        _dbContext.SensorEvents.AddRange(events);
        _dbContext.SaveChanges();
    }

    private static SensorEvent Event(string type, string name) => new()
    {
        Id = Guid.NewGuid(),
        Type = type,
        Name = name,
        Payload = JsonDocument.Parse("""{"value":1}""").RootElement.Clone(),
        ReceivedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
