using DataIngestorService.APIs;
using DataIngestorService.APIs.DelegatingHandlers;
using DataIngestorService.BackgroundServices;
using DataIngestorService.ConfigurationOptions;
using DataIngestorService.MessagingService;
using DataIngestorService.RetryPolicies;
using Microsoft.Extensions.Options;
using Refit;
using Host = Microsoft.Extensions.Hosting.Host;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<SensorDataFetcherService>();

var configuration = builder.Configuration;

builder.Services
    .AddOptions<WeakAppOptions>()
    .Bind(configuration.GetSection(WeakAppOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole(); 
});

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
app.Run();
