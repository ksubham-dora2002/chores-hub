using AutoMapper;
using ChoresHub.Domain.Entities;
using ChoresHub.Application.DTOs;
using ChoresHub.Domain.Interfaces;
using ChoresHub.Application.Common;
using ChoresHub.Application.Interfaces;
using ChoresHub.Application.Helpers;

namespace ChoresHub.Application.Services
{
    public class NotificationService(INotificationRepository notificationRepository, IMapper mapper, IUserRepository userRepository) : INotificationService
    {
        private const int _defaultPageSize = 10;
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        public async Task<Result<string>> DeleteNotificationAsync(Guid id)
        {
            var query = await _notificationRepository.GetByIdAsync(id);
            if (query == null) return Result<string>.Failure("Notification not found!");

            await _notificationRepository.DeleteAsync(id);
            return Result<string>.Success("Notification deleted successfully!");
        }
        public async Task<Result<NotificationDTO?>> GetNotificationByIdAsync(Guid id)
        {
            var query = await _notificationRepository.GetByIdAsync(id);
            if (query == null) return Result<NotificationDTO?>.Failure("Notification not found!");
            return Result<NotificationDTO?>.Success(_mapper.Map<NotificationDTO>(query));
        }
        public async Task<Result<IList<NotificationDTO>>> GetAllNotificationsAsync(int pageSize = _defaultPageSize)
        {


            var query = await _notificationRepository.GetAllAsync();
            if (query == null || !query.Any())
                return Result<IList<NotificationDTO>>.Failure("No notifications found!");

            var result = query
                .Where(n => n.IsSeen == false)
                .Take(pageSize)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    Content = n.Content,
                    IsSeen = n.IsSeen,
                    CreatedAt = n.CreatedAt,
                    UserId = n.UserId,
                    UserName = n.User.FullName,
                    UserPicture = n.User.ProfilePictureUrl!
                }).ToList();

            return Result<IList<NotificationDTO>>.Success(result);
        }
        public async Task<Result<NotificationDTO?>> UpdateNotificationAsync(NotificationDTO notificationDTO)
        {
            var query = await _notificationRepository.GetByIdAsync(notificationDTO.Id);
            if (query == null) return Result<NotificationDTO?>.Failure("Notification not found!");

            query.Content = Utilities.CapitalizeFirstLetter(notificationDTO.Content);
            query.CreatedAt = notificationDTO.CreatedAt;
            query.IsSeen = notificationDTO.IsSeen;

            var user = await _userRepository.GetByIdAsync(notificationDTO.UserId);
            query.UserId = notificationDTO.UserId;
            if (user == null) return Result<NotificationDTO?>.Failure(UserMessages.UserNotFoundById);
            query.User = user;

            await _notificationRepository.UpdateAsync(query);

            var updatedDbNotification = await GetNotificationByIdAsync(query.Id);
            if (updatedDbNotification.Value == null) return Result<NotificationDTO?>.Failure("Error occurred while updating the notification!");

            return Result<NotificationDTO?>.Success(updatedDbNotification.Value);

        }
        public async Task<Result<NotificationDTO?>> CreateNotificationAsync(CreateNotificationDTO notificationDTO)
        {
            var user = await _userRepository.GetByIdAsync(notificationDTO.UserId);
            if (user == null) return Result<NotificationDTO?>.Failure(UserMessages.UserNotFoundById);
            var newNotification = new Notification
            {
                Id = Guid.NewGuid(),
                Content = Utilities.CapitalizeFirstLetter(notificationDTO.Content),
                IsSeen = notificationDTO.IsSeen,
                CreatedAt = notificationDTO.CreatedAt,
                UserId = notificationDTO.UserId,
                User = user
            };
            await _notificationRepository.CreateAsync(newNotification);

            var query = await _notificationRepository.GetByIdAsync(newNotification.Id);
            if (query == null) return Result<NotificationDTO?>.Failure("Error occurred while creating the notification!");

            return Result<NotificationDTO?>.Success(_mapper.Map<NotificationDTO>(query));
        }
        public async Task<Result<IList<NotificationDTO>>> GetAllNotificationsByUserEmailAsync(string email, int pageSize = _defaultPageSize)
        {

            var query = await _notificationRepository.GetAllNotificationsByUserEmailAsync(email);

            if (query == null || !query.Any()) return Result<IList<NotificationDTO>>.Success(new List<NotificationDTO>());

            var result = query
                .Take(pageSize)
                .Select(n => new NotificationDTO
                {
                    Id = n!.Id,
                    Content = n.Content,
                    IsSeen = n.IsSeen,
                    CreatedAt = n.CreatedAt,
                    UserId = n.UserId,
                    UserName = n.User?.FullName ?? "Unknown",
                    UserPicture = n.User?.ProfilePictureUrl ?? string.Empty
                }).ToList();



            return Result<IList<NotificationDTO>>.Success(result);
        }

    }
}