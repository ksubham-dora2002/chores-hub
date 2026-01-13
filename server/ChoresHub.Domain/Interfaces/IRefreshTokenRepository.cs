using SystemTask = System.Threading.Tasks.Task;
using ChoresHub.Domain.Entities;

namespace ChoresHub.Domain.Interfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        SystemTask UpdateAsyncRefreshToken(RefreshToken token);
        Task<IList<RefreshToken>> GetTokensByEmailAsync(string email);
        Task<RefreshToken> FindRefreshTokenByTokenAsync(string token);
    }
}