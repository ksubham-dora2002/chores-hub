using TaskEntity = ChoresHub.Domain.Entities.Task;

namespace ChoresHub.Domain.Interfaces
{
    public interface ITaskRepository : IGenericRepository<TaskEntity>
    {
        Task<IList<TaskEntity>> GetAllTasksByUserEmailAsync(string email);
    }
}