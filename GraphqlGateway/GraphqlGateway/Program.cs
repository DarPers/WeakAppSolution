using GraphqlGateway.Data;
using GraphqlGateway.Logging;
using HotChocolate.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddStructuredSerilogLogging();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(option =>
    option.UseNpgsql(configuration.GetConnectionString("PostgresSQLConnectionString")));

builder.Services
    .AddGraphQLServer()
    .AddQueryType<GraphqlGateway.Schema.Queries>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter());

try
{
    var app = builder.Build();

    app.MapPrometheusScrapingEndpoint();

    app.MapGraphQL().WithOptions(new GraphQLServerOptions
    {
        Tool = { Enable = app.Environment.IsDevelopment() }
    });

    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
