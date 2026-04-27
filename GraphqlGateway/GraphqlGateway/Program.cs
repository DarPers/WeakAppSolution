using GraphqlGateway.Data;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
        .AddAspNetCoreInstrumentation() // Метрики запросов
        .AddRuntimeInstrumentation()    // Память, GC, CPU
        .AddPrometheusExporter());

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

app.MapPrometheusScrapingEndpoint();

app.MapGraphQL();

app.Run();
