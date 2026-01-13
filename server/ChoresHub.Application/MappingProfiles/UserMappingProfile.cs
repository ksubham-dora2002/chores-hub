using AutoMapper;
using ChoresHub.Domain.Entities;
using ChoresHub.Application.DTOs;

namespace ChoresHub.Application.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, LoginDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();
        }
    }
}