using ChoresHub.Application.DTOs;
using ChoresHub.Application.Common;

namespace ChoresHub.Application.Interfaces
{
    public interface INotificationService
    {
        Task<Result<string>> DeleteNotificationAsync(Guid id);
        Task<Result<NotificationDTO?>> GetNotificationByIdAsync(Guid id);
        Task<Result<IList<NotificationDTO>>> GetAllNotificationsAsync(int pageSize = 10);
        Task<Result<NotificationDTO?>> UpdateNotificationAsync(NotificationDTO notificationDTO);
        Task<Result<NotificationDTO?>> CreateNotificationAsync(CreateNotificationDTO notificationDTO);
        Task<Result<IList<NotificationDTO>>> GetAllNotificationsByUserEmailAsync(string email, int pageSize = 10);
    }
}