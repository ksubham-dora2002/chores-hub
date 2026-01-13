using SystemTask = System.Threading.Tasks.Task;
using ChoresHub.Domain.Entities;

namespace ChoresHub.Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        SystemTask DeleteAsync(Guid id);
        Task<TEntity?> GetByIdAsync(Guid id);
        SystemTask CreateAsync(TEntity entity);
        SystemTask UpdateAsync(TEntity entity);
        Task<IQueryable<TEntity>> GetAllAsync();
    }
}