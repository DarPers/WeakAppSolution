using Serilog;
using Serilog.Core;

namespace DataIngestorService.Logging;

public static class LoggingServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddStructuredSerilogLogging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ILoggingContextAccessor, LoggingContextAccessor>();
        builder.Services.AddSingleton<ILogEventEnricher, LoggingContextEnricher>();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateBootstrapLogger();

        builder.Services.AddSerilog((services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });

        return builder;
    }
}
