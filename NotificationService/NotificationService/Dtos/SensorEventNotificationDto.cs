using System.Text.Json;

namespace NotificationService.Dtos;

public sealed class SensorEventNotificationDto
{
    public required string Type { get; init; }

    public required string Name { get; init; }

    public required JsonElement Payload { get; init; }
}
