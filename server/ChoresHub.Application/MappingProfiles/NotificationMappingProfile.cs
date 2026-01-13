using AutoMapper;
using ChoresHub.Domain.Entities;
using ChoresHub.Application.DTOs;

namespace ChoresHub.Application.MappingProfiles
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<Notification, NotificationDTO>().ReverseMap();
            CreateMap<Notification, CreateNotificationDTO>().ReverseMap();
        }
    }
}