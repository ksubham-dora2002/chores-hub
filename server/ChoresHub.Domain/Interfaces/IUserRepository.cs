using ChoresHub.Domain.Entities;

namespace ChoresHub.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        
    }
}