using AutoMapper;
using ChoresHub.Application.DTOs;
using ChoresHub.Domain.Interfaces;
using ChoresHub.Application.Common;
using ChoresHub.Application.Interfaces;
using TaskEntity = ChoresHub.Domain.Entities.Task;
using ChoresHub.Application.Helpers;

namespace ChoresHub.Application.Services
{
    public class TaskService(ITaskRepository taskRepository, IMapper mapper, IUserRepository userRepository) : ITaskService
    {
        private const int _defaultPageSize = 10;
        private readonly IMapper _mapper = mapper;
        private readonly ITaskRepository _taskRepository = taskRepository;
        private readonly IUserRepository _userRepository = userRepository;
        public async Task<Result<string>> DeleteTaskAsync(Guid id)
        {
            var query = await _taskRepository.GetByIdAsync(id);
            if (query== null) return Result<string>.Failure("Task not found!");

            await _taskRepository.DeleteAsync(id);
            return Result<string>.Success("Task deleted successfully!");
        }

        public async Task<Result<TaskDTO?>> GetTaskByIdAsync(Guid id)
        {
            var query = await _taskRepository.GetByIdAsync(id);
            if (query == null) return Result<TaskDTO?>.Failure("Task not found!");
            return Result<TaskDTO?>.Success(_mapper.Map<TaskDTO>(query));
        }

        public async Task<Result<TaskDTO?>> UpdateTaskAsync(TaskDTO taskDTO)
        {
            var query = await _taskRepository.GetByIdAsync(taskDTO.Id);
            if (query == null) return Result<TaskDTO?>.Failure("Task not found!");

            query.Content = Utilities.CapitalizeFirstLetter(taskDTO.Content);
            query.CreatedAt = taskDTO.CreatedAt;
            query.IsDone = taskDTO.IsDone;

            var user = await _userRepository.GetByIdAsync(taskDTO.UserId);
            query.UserId = taskDTO.UserId;
            if (user == null) return Result<TaskDTO?>.Failure(UserMessages.UserNotFoundById);
            query.User = user;

            await _taskRepository.UpdateAsync(query);

            var updatedDbTask = await GetTaskByIdAsync(query.Id);
            if (updatedDbTask.Value == null) return Result<TaskDTO?>.Failure("Error occurred while updating the task!");

            return Result<TaskDTO?>.Success(updatedDbTask.Value);
        }
        public async Task<Result<TaskDTO?>> CreateTaskAsync(CreateTaskDTO taskDTO)
        {
            var user = await _userRepository.GetByIdAsync(taskDTO.UserId);
            if (user == null) return Result<TaskDTO?>.Failure(UserMessages.UserNotFoundById);
            var newTask = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Content = Utilities.CapitalizeFirstLetter(taskDTO.Content),
                CreatedAt = taskDTO.CreatedAt,
                IsDone = taskDTO.IsDone,
                UserId = taskDTO.UserId,
                User = user
            };
            await _taskRepository.CreateAsync(newTask);

            var query = await _taskRepository.GetByIdAsync(newTask.Id);
            if (query == null) return Result<TaskDTO?>.Failure("Error occurred while creating the task!");
            
            return Result<TaskDTO?>.Success(_mapper.Map<TaskDTO>(query));
        }
        public async Task<Result<IList<TaskDTO>>> GetAllTasksAsync(int pageSize = _defaultPageSize)
        {
            var query = await _taskRepository.GetAllAsync();
            if (query == null || !query.Any())
                return Result<IList<TaskDTO>>.Success(new List<TaskDTO>());

            var result = query
                .Where(s => s.IsDone == false)
                .Take(pageSize)
                .Select(t => new TaskDTO
                {
                    Id = t.Id,
                    Content = t.Content,
                    IsDone = t.IsDone,
                    CreatedAt = t.CreatedAt,
                    UserId = t.UserId,
                    UserName = t.User.FullName,
                    UserPicture = t.User.ProfilePictureUrl!
                }).ToList();
            return Result<IList<TaskDTO>>.Success(result);
        }
        public async Task<Result<IList<TaskDTO>>> GetAllTasksByUserEmailAsync(string email, int pageSize = _defaultPageSize)
        {
            var query = await _taskRepository.GetAllTasksByUserEmailAsync(email);
            if (query == null || !query.Any()) return Result<IList<TaskDTO>>.Success(new List<TaskDTO>());

            var result = query
                .Take(pageSize)
                .Select(t => new TaskDTO
                {
                    Id = t.Id,
                    Content = t.Content,
                    IsDone = t.IsDone,
                    CreatedAt = t.CreatedAt,
                    UserId = t.UserId,
                    UserName = t.User?.FullName ?? "Unknown",
                    UserPicture = t.User?.ProfilePictureUrl ?? string.Empty
                }).ToList();

            return Result<IList<TaskDTO>>.Success(result);
        }

    }
}