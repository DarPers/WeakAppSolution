using NotificationService.Dtos;

namespace NotificationService.Hubs;

public interface ISensorEventsClient
{
    Task SensorDataUpdated(SensorNotificationDto notification);
}
