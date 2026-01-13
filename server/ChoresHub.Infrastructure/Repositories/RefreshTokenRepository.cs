using ChoresHub.Domain.Entities;
using ChoresHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChoresHub.Infrastructure.Contexts;
using SystemTask = System.Threading.Tasks.Task;

namespace ChoresHub.Infrastructure.Repositories
{
    public class RefreshTokenRepository(PostgreDbContext postgreDbContext) : GenericRepository<RefreshToken>(postgreDbContext), IRefreshTokenRepository
    {
        private readonly PostgreDbContext _contxt = postgreDbContext;



        public async Task<RefreshToken?> FindRefreshTokenByTokenAsync(string refToken)
        {
            return await _contxt.Set<RefreshToken>()
                .FirstOrDefaultAsync(r => r.Token == refToken && !r.IsRevoked);
        }

        public async Task<IList<RefreshToken>> GetTokensByEmailAsync(string email)
        {
            return await _contxt.Set<RefreshToken>().Where(r => r.UserEmail == email).ToListAsync();
        }

        public async SystemTask UpdateAsyncRefreshToken(RefreshToken token)
        {
            _contxt.RefreshTokens.Update(token);
            await _contxt.SaveChangesAsync();
        }
    }
}