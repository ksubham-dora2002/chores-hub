using ChoresHub.Domain.Entities;
using ChoresHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChoresHub.Infrastructure.Contexts;

namespace ChoresHub.Infrastructure.Repositories
{
    public class ShoppingProductRepository(PostgreDbContext postgreDbContext) : GenericRepository<ShoppingProduct>(postgreDbContext), IShoppingProductRepository
    {
        private readonly PostgreDbContext _contxt = postgreDbContext;

        public async Task<IList<ShoppingProduct>> GetAllShopProductsByUserEmailAsync(string email)
        {
            var user = await _contxt.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
            return await _contxt.Set<ShoppingProduct>()
                .Where(n => n.UserId == user!.Id)
                .ToListAsync();
        }

    }
}