using DataProcessorService;
using DataProcessorService.DAL;
using DataProcessorService.Logging;
using DataProcessorService.Telemetry;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddStructuredSerilogLogging();

builder.Services.ConfigureMessagingServices(builder.Configuration);
builder.Services.ConfigureDALServices(builder.Configuration);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("data-processor-service"))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddMeter(ServiceTelemetry.MeterName)
        .AddPrometheusExporter())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddSource(ServiceTelemetry.ActivitySourceName));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

try
{
    app.MapPrometheusScrapingEndpoint();
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
