using GraphqlGateway.Data;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();

app.MapGraphQL();

app.Run();
