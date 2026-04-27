using System.Text.Json;

namespace GraphqlGateway.Models;

public class SensorEvent
{
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public required JsonElement Payload { get; set; }

    public DateTime ReceivedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}