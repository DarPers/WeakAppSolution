using DataProcessorService.DAL.ConfigurationOptions;
using DataProcessorService.DAL.Interfaces;
using DataProcessorService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataProcessorService.DAL;

public static class DataAccessCollectionExtensions
{
    public static IServiceCollection ConfigureDALServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<PostgresOptions>()
            .Bind(configuration.GetSection(PostgresOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<ApplicationDbContext>((sp, option) =>
        {
            var postgres = sp.GetRequiredService<IOptions<PostgresOptions>>().Value;
            option.UseNpgsql(postgres.PostgresSQLConnectionString);
        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ISensorEventRepository, SensorEventRepository>();

        return services;
    }
}
