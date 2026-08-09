using DataProcessorService;
using DataProcessorService.DAL;
using DataProcessorService.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Host = Microsoft.Extensions.Hosting.Host;

var builder = Host.CreateApplicationBuilder(args);

builder.AddStructuredSerilogLogging();

builder.Services.ConfigureMessagingServices(builder.Configuration);
builder.Services.ConfigureDALServices(builder.Configuration);

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

try
{
    host.Run();
}
finally
{
    Log.CloseAndFlush();
}
