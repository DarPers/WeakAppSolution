using FluentAssertions;
using GraphqlGateway.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GraphqlGateway.Tests.Logging;

public class LoggingServiceCollectionExtensionsTests
{
    [Fact]
    public void AddStructuredSerilogLogging_ConfiguresSerilogAndReturnsBuilder()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Serilog:MinimumLevel:Default"] = "Information",
            ["Serilog:WriteTo:0:Name"] = "Console"
        });

        var result = builder.AddStructuredSerilogLogging();

        result.Should().BeSameAs(builder);
        Log.Logger.Should().NotBe(Serilog.Core.Logger.None);

        using var app = builder.Build();
        app.Services.GetService<ILoggerFactory>().Should().NotBeNull();
        app.Services.GetService<Serilog.ILogger>().Should().NotBeNull();
    }
}
