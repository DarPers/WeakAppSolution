using DataIngestorService.Contracts.Contracts;
using NotificationService.Dtos;

namespace NotificationService.Mapping;

public static class SensorNotificationMapper
{
    public static SensorNotificationDto ToNotification(SensorEventsMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(message.SensorEvents);

        return new SensorNotificationDto
        {
            ReceivedAt = message.ReceivedAt,
            Events = message.SensorEvents
                .Select(e => new SensorEventNotificationDto
                {
                    Type = e.Type,
                    Name = e.Name,
                    Payload = e.Payload
                })
                .ToList()
        };
    }
}
