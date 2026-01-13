using ChoresHub.Domain.Entities;
using ChoresHub.Domain.Interfaces;
using ChoresHub.Infrastructure.Contexts;
using SystemTask = System.Threading.Tasks.Task;

namespace ChoresHub.Infrastructure.Repositories
{
    public class GenericRepository<TEntity>(PostgreDbContext postgreDbContext) : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly PostgreDbContext _contxt = postgreDbContext;

        public async SystemTask CreateAsync(TEntity entity)
        {
            await _contxt.Set<TEntity>().AddAsync(entity);
            await _contxt.SaveChangesAsync();
        }

        public async SystemTask DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _contxt.Set<TEntity>().Remove(entity);
                await _contxt.SaveChangesAsync();
            }
        }

        public async Task<IQueryable<TEntity>> GetAllAsync()
        {
            return await SystemTask.FromResult(_contxt.Set<TEntity>().AsQueryable());
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _contxt.Set<TEntity>().FindAsync(id);
        }

        public async SystemTask UpdateAsync(TEntity entity)
        {
            _contxt.Set<TEntity>().Update(entity);
            await _contxt.SaveChangesAsync();
        }
    }
}