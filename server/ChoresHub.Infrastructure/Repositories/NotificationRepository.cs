using ChoresHub.Domain.Entities;
using ChoresHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChoresHub.Infrastructure.Contexts;

namespace ChoresHub.Infrastructure.Repositories
{
    public class NotificationRepository(PostgreDbContext postgreDbContext) : GenericRepository<Notification>(postgreDbContext), INotificationRepository
    {
        private readonly PostgreDbContext _contxt = postgreDbContext;

        public async Task<IList<Notification>> GetAllNotificationsByUserEmailAsync(string email)
        {
            var user = await _contxt.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
            return await _contxt.Set<Notification>()
                .Where(n => n!.UserId == user!.Id)
                .ToListAsync();
        }

    }
}