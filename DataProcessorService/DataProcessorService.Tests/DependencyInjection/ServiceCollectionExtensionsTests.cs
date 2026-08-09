using DataProcessorService.ConfigurationOptions;
using DataProcessorService.DAL;
using DataProcessorService.DAL.ConfigurationOptions;
using DataProcessorService.DAL.Interfaces;
using DataProcessorService.Logging;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DataProcessorService.Tests.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddStructuredSerilogLogging_RegistersContextAndEnricher()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Serilog:MinimumLevel:Default"] = "Information"
        });

        builder.AddStructuredSerilogLogging();

        using var host = builder.Build();
        host.Services.GetService<ILoggingContextAccessor>().Should().NotBeNull();
        host.Services.GetServices<Serilog.Core.ILogEventEnricher>()
            .OfType<LoggingContextEnricher>()
            .Should()
            .ContainSingle();
    }

    [Fact]
    public void ConfigureDALServices_RegistersPostgresOptionsAndRepository()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgresSQLConnectionString"] =
                    "Host=localhost;Port=5432;Database=testdb;Username=postgres"
            })
            .Build();

        var services = new ServiceCollection();
        services.ConfigureDALServices(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<PostgresOptions>>().Value;

        options.PostgresSQLConnectionString.Should().Contain("testdb");
        provider.GetService<ISensorEventRepository>().Should().NotBeNull();
    }

    [Fact]
    public void ConfigureMessagingServices_RegistersValidatedRabbitMqOptions()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RabbitMQ:HostName"] = "rabbitmq",
                ["RabbitMQ:VirtualHostName"] = "/",
                ["RabbitMQ:UserName"] = "guest",
                ["RabbitMQ:Password"] = "guest",
                ["RabbitMQ:QueueName"] = "processor-sensor-events",
                ["RabbitMQ:ConsumeRetryCount"] = "3",
                ["RabbitMQ:ConsumeRetryDelayMilliseconds"] = "1000"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.ConfigureMessagingServices(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

        options.QueueName.Should().Be("processor-sensor-events");
        options.ConsumeRetryCount.Should().Be(3);
    }
}
