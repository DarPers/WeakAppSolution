using System.Text.Json;

namespace DataIngestorService.Contracts.Contracts;

public class SensorEvent
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public required JsonElement Payload { get; set; }
}
