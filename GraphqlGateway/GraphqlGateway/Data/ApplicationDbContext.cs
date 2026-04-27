using GraphqlGateway.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphqlGateway.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<SensorEvent> SensorEvents { get; set; }
}
