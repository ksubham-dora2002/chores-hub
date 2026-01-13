using ChoresHub.Domain.Entities;

namespace ChoresHub.Domain.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IList<Notification>> GetAllNotificationsByUserEmailAsync(string email);
    }
}