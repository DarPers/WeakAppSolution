using DataProcessorService.DAL.Interfaces;
using DataProcessorService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataProcessorService.DAL;

public static class DataAccessCollectionExtensions
{
    public static IServiceCollection ConfigureDALServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(option =>
            option.UseNpgsql(configuration.GetConnectionString("PostgresSQLConnectionString")));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ISensorEventRepository, SensorEventRepository>();

        return services;
    }
}
