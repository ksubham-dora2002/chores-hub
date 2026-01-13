using ChoresHub.Application.DTOs;
using ChoresHub.Application.Common;

namespace ChoresHub.Application.Interfaces
{
    public interface IShoppingProductService
    {
        Task<Result<string>> DeleteShopProductAsync(Guid id);
        Task<Result<ShoppingProductDTO?>> GetShopProductByIdAsync(Guid id);
        Task<Result<IList<ShoppingProductDTO>>> GetAllShopProductsAsync(int pageSize = 10);
        Task<Result<ShoppingProductDTO?>> UpdateShopProductAsync(ShoppingProductDTO shopProductDTO);
        Task<Result<ShoppingProductDTO?>> CreateShopProductAsync(CreateShoppingProductDTO shopProductDTO);
        Task<Result<IList<ShoppingProductDTO>>> GetAllShopProductsByUserEmailAsync(string email, int pageSize = 10);
    }
}