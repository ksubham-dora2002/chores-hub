using ChoresHub.Application.DTOs;
using ChoresHub.Application.Common;

namespace ChoresHub.Application.Interfaces
{
    public interface ITaskService
    {
        Task<Result<string>> DeleteTaskAsync(Guid id);
        Task<Result<TaskDTO?>> GetTaskByIdAsync(Guid id);
        Task<Result<TaskDTO?>> UpdateTaskAsync(TaskDTO taskDTO);
        Task<Result<TaskDTO?>> CreateTaskAsync(CreateTaskDTO taskDTO);
        Task<Result<IList<TaskDTO>>> GetAllTasksAsync(int pageSize = 10);
        Task<Result<IList<TaskDTO>>> GetAllTasksByUserEmailAsync(string email, int pageSize = 10);

    }
}