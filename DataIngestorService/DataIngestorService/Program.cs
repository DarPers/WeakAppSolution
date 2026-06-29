using DataIngestorService.APIs;
using DataIngestorService.APIs.DelegatingHandlers;
using DataIngestorService.BackgroundServices;
using DataIngestorService.ConfigurationOptions;
using DataIngestorService.Logging;
using DataIngestorService.MessagingService;
using DataIngestorService.RetryPolicies;
using Microsoft.Extensions.Options;
using Refit;
using Serilog;
using Host = Microsoft.Extensions.Hosting.Host;

var builder = Host.CreateApplicationBuilder(args);

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

var app = builder.Build();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
