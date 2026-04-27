using GraphqlGateway.Data;
using GraphqlGateway.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphqlGateway.Schema;

public class Queries
{
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<SensorEvent> GetSensorEvents([Service] ApplicationDbContext dbContext) =>
        dbContext.SensorEvents;

    public record TypeStats(string Type, int Count);

    public IQueryable<TypeStats> GetStatsByType([Service] ApplicationDbContext dbContext)
    {
        return dbContext.SensorEvents
            .GroupBy(x => x.Type)
            .Select(g => new TypeStats(g.Key, g.Count()));
    }

    public IQueryable<TypeStats> GetStatsByLocation([Service] ApplicationDbContext dbContext)
    {
        return dbContext.SensorEvents
            .GroupBy(x => x.Name)
            .Select(g => new TypeStats(g.Key, g.Count()));
    }
}
