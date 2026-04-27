using System.Text.Json;

namespace DataProcessorService.DAL.Entities;

public class SensorEvent : BaseEntity
{
    public string Type { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public required JsonElement Payload { get; set; }

    public DateTime ReceivedAt { get; set; }
}
