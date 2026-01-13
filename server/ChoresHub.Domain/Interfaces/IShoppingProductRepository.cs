using ChoresHub.Domain.Entities;

namespace ChoresHub.Domain.Interfaces
{
    public interface IShoppingProductRepository : IGenericRepository<ShoppingProduct>
    {
        Task<IList<ShoppingProduct>> GetAllShopProductsByUserEmailAsync(string email);
    }
}