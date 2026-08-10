using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DataProcessorService.Telemetry;

public static class ServiceTelemetry
{
    public const string MeterName = "DataProcessorService";
    public const string ActivitySourceName = "DataProcessorService";

    public static readonly Meter Meter = new(MeterName);
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static readonly Counter<long> EventsPersisted =
        Meter.CreateCounter<long>("sensor.events.persisted", unit: "{event}", description: "Sensor events persisted to PostgreSQL");

    public static readonly Counter<long> PersistErrors =
        Meter.CreateCounter<long>("sensor.persist.errors", unit: "{error}", description: "Failed sensor event persist attempts");
}
