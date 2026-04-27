using DataProcessorService.DAL.Entities;

namespace DataProcessorService.DAL.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<List<T>> Create(List<T> entities);

    Task<T> Update(T entity);

    Task Delete(T entity);

    Task<List<T>> GetAll();

    Task<T?> GetById(Guid entityId);
}
