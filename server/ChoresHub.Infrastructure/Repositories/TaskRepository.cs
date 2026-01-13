using ChoresHub.Domain.Entities;
using ChoresHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChoresHub.Infrastructure.Contexts;
using TaskEntity = ChoresHub.Domain.Entities.Task;

namespace ChoresHub.Infrastructure.Repositories
{
    public class TaskRepository(PostgreDbContext postgreDbContext) : GenericRepository<TaskEntity>(postgreDbContext), ITaskRepository
    {
        private readonly PostgreDbContext _contxt = postgreDbContext;

        public async Task<IList<TaskEntity>> GetAllTasksByUserEmailAsync(string email)
        {
            var user = await _contxt.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
            return await _contxt.Set<TaskEntity>()
                .Where(n => n!.UserId == user!.Id)
                .ToListAsync();
        }

    }
}