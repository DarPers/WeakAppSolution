using DataProcessorService.DAL.Entities;
using DataProcessorService.DAL.Interfaces;

namespace DataProcessorService.DAL.Repositories;

public class SensorEventRepository(ApplicationDbContext dbContext) : GenericRepository<SensorEvent>(dbContext), ISensorEventRepository
{
}
