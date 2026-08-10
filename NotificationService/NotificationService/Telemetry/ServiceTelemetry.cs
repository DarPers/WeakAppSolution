using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace NotificationService.Telemetry;

public static class ServiceTelemetry
{
    public const string MeterName = "NotificationService";
    public const string ActivitySourceName = "NotificationService";

    public static readonly Meter Meter = new(MeterName);
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static readonly Counter<long> EventsBroadcast =
        Meter.CreateCounter<long>("sensor.events.broadcast", unit: "{event}", description: "Sensor events broadcast via SignalR");

    public static readonly Counter<long> BroadcastErrors =
        Meter.CreateCounter<long>("sensor.broadcast.errors", unit: "{error}", description: "Failed SignalR sensor broadcasts");
}
