using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DataIngestorService.Telemetry;

public static class ServiceTelemetry
{
    public const string MeterName = "DataIngestorService";
    public const string ActivitySourceName = "DataIngestorService";

    public static readonly Meter Meter = new(MeterName);
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static readonly Counter<long> EventsFetched =
        Meter.CreateCounter<long>("sensor.events.fetched", unit: "{event}", description: "Sensor events fetched from WeakApp");

    public static readonly Counter<long> EventsPublished =
        Meter.CreateCounter<long>("sensor.events.published", unit: "{event}", description: "Sensor events published to RabbitMQ");

    public static readonly Counter<long> FetchErrors =
        Meter.CreateCounter<long>("sensor.fetch.errors", unit: "{error}", description: "Failed WeakApp fetch or publish iterations");
}
