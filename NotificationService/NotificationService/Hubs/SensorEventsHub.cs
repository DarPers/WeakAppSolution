using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Hubs;

public class SensorEventsHub : Hub<ISensorEventsClient>
{
}
