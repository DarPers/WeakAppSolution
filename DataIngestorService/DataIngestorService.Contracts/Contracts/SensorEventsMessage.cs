namespace DataIngestorService.Contracts.Contracts;

public class SensorEventsMessage
{
    public required List<SensorEvent> SensorEvents { get; set; }
    public DateTime ReceivedAt { get; set; }
}

