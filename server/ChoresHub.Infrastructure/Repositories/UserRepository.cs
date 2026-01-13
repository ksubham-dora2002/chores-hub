using ChoresHub.Domain.Entities;
using ChoresHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChoresHub.Infrastructure.Contexts;

namespace ChoresHub.Infrastructure.Repositories
{
    public class UserRepository(PostgreDbContext postgreDbContext) : GenericRepository<User>(postgreDbContext), IUserRepository
    {
        private readonly PostgreDbContext _contxt = postgreDbContext;


        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _contxt.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

    }
}