using DataProcessorService.DAL.Entities;
using DataProcessorService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace DataProcessorService.DAL.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;

    private readonly DbSet<T> _entities;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _entities = _context.Set<T>();
    }

    public async Task<List<T>> Create(List<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return entities;
    }

    public async Task Delete(T entity)
    {
        _entities.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public Task<List<T>> GetAll()
    {
        return _entities.ToListAsync();
    }

    public Task<T?> GetById(Guid entityId)
    {
        return _entities.AsNoTracking().FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<T> Update(T entity)
    {
        _entities.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}