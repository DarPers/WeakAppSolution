using DataIngestorService.APIs;
using DataIngestorService.APIs.DelegatingHandlers;
using DataIngestorService.BackgroundServices;
using DataIngestorService.ConfigurationOptions;
using DataIngestorService.Logging;
using DataIngestorService.MessagingService;
using DataIngestorService.RetryPolicies;
using DataIngestorService.Telemetry;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Refit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddStructuredSerilogLogging();

builder.Services.AddHostedService<SensorDataFetcherService>();

var configuration = builder.Configuration;

builder.Services
    .AddOptions<WeakAppOptions>()
    .Bind(configuration.GetSection(WeakAppOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddTransient<WeakApiAuthHandler>();

builder.Services
    .AddRefitClient<IWeakAppApi>()
    .ConfigureHttpClient((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<WeakAppOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl);
    })
    .AddHttpMessageHandler<WeakApiAuthHandler>()
    .AddPolicyHandler(RetryPoliciesExtentions.GetRetryPolicy())
    .AddPolicyHandler((sp, request) =>
    {
        var logger = sp.GetRequiredService<ILogger<WeakApiAuthHandler>>();
        return RetryPoliciesExtentions.GetCircuitBreakerPolicy(logger);
    });

builder.Services.AddMessageServices(configuration);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("data-ingestor-service"))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddMeter(ServiceTelemetry.MeterName)
        .AddPrometheusExporter())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource(ServiceTelemetry.ActivitySourceName));

try
{
    var app = builder.Build();

    app.MapPrometheusScrapingEndpoint();
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
