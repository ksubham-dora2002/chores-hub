using AutoMapper;
using ChoresHub.Application.DTOs;
using TaskEntity = ChoresHub.Domain.Entities.Task;

namespace ChoresHub.Application.MappingProfiles
{
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<TaskEntity, TaskDTO>().ReverseMap();
            CreateMap<TaskEntity, CreateTaskDTO>().ReverseMap();

        }
    }
}