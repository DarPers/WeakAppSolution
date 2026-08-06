namespace NotificationService.Dtos;

public sealed class SensorNotificationDto
{
    public required IReadOnlyList<SensorEventNotificationDto> Events { get; init; }

    public DateTime ReceivedAt { get; init; }
}
