using ChoresHub.Domain.Entities;
using SystemTask = System.Threading.Tasks.Task;

namespace ChoresHub.Domain.Interfaces
{
    public interface IPasswordResetTokenRepository : IGenericRepository<PasswordResetToken>
    {
        Task<PasswordResetToken?> FindActiveTokenByHashAsync(string tokenHash);
        SystemTask InvalidateTokensForUserAsync(Guid userId);
    }
}
