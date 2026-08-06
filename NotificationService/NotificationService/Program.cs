using MassTransit;
using Microsoft.Extensions.Options;
using NotificationService.Consumers;
using NotificationService.Hubs;
using NotificationService.Logging;
using NotificationService.Options;
using Serilog;
using CorsOptions = NotificationService.Options.CorsOptions;

var builder = WebApplication.CreateBuilder(args);

builder.AddStructuredSerilogLogging();

var configuration = builder.Configuration;

builder.Services.AddSignalR();

builder.Services
    .AddOptions<RabbitMqOptions>()
    .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<CorsOptions>()
    .Bind(configuration.GetSection(CorsOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<BroadcastOptions>()
    .Bind(configuration.GetSection(BroadcastOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumers(typeof(Program).Assembly);

    config.UsingRabbitMq((context, connectionSettings) =>
    {
        var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

        connectionSettings.Host(options.HostName, options.VirtualHostName, h =>
        {
            h.Username(options.UserName);
            h.Password(options.Password);
        });

        connectionSettings.ReceiveEndpoint(options.QueueName, ep =>
        {
            ep.ConfigureConsumer<SensorMessageConsumer>(context);
        });
    });
});

builder.Services.AddCors();

try
{
    var app = builder.Build();

    var corsOptions = app.Services.GetRequiredService<IOptions<CorsOptions>>().Value;

    app.UseCors(policy =>
    {
        policy.WithOrigins(corsOptions.AllowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    app.MapHub<SensorEventsHub>("/hubs/sensors");
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
