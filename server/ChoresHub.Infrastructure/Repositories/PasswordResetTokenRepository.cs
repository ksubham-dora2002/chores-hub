using ChoresHub.Domain.Entities;
using ChoresHub.Domain.Interfaces;
using ChoresHub.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using SystemTask = System.Threading.Tasks.Task;

namespace ChoresHub.Infrastructure.Repositories
{
    public class PasswordResetTokenRepository(PostgreDbContext db) : GenericRepository<PasswordResetToken>(db), IPasswordResetTokenRepository
    {
        private readonly PostgreDbContext _contxt = db;

        public async Task<PasswordResetToken?> FindActiveTokenByHashAsync(string tokenHash)
        {
            return await _contxt.Set<PasswordResetToken>()
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash &&
                    x.UsedAt == null &&
                    x.ExpiresAt > DateTime.UtcNow);
        }


        public async SystemTask InvalidateTokensForUserAsync(Guid userId)
        {
            var tokens = await _contxt.Set<PasswordResetToken>()
                .Where(x => x.UserId == userId && x.UsedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.UsedAt = DateTime.UtcNow;
            }

            await _contxt.SaveChangesAsync();
        }
    }
}
