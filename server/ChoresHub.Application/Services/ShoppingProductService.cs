using AutoMapper;
using ChoresHub.Domain.Entities;
using ChoresHub.Application.DTOs;
using ChoresHub.Domain.Interfaces;
using ChoresHub.Application.Common;
using ChoresHub.Application.Interfaces;
using ChoresHub.Application.Helpers;

namespace ChoresHub.Application.Services
{
    public class ShoppingProductService(IShoppingProductRepository shoppingProductRepository, IMapper mapper, IUserRepository userRepository) : IShoppingProductService
    {
        private const int _defaultPageSize = 10;
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IShoppingProductRepository _shoppingProductRepository = shoppingProductRepository;

        public async Task<Result<string>> DeleteShopProductAsync(Guid id)
        {
            var product = await _shoppingProductRepository.GetByIdAsync(id);
            if (product == null) return Result<string>.Failure("Shopping product not found!");
    
            await _shoppingProductRepository.DeleteAsync(id);
            return Result<string>.Success("Shopping product deleted successfully!");
        }
        public async Task<Result<ShoppingProductDTO?>> GetShopProductByIdAsync(Guid id)
        {
            var query = await _shoppingProductRepository.GetByIdAsync(id);
            if (query == null) return Result<ShoppingProductDTO?>.Failure("Shopping product not found!");
            return Result<ShoppingProductDTO?>.Success(_mapper.Map<ShoppingProductDTO>(query));
        }


        public async Task<Result<IList<ShoppingProductDTO>>> GetAllShopProductsAsync(int pageSize = _defaultPageSize)
        {
            var query = await _shoppingProductRepository.GetAllAsync();
            if (query == null || !query.Any())
                return Result<IList<ShoppingProductDTO>>.Failure("No shopping products found!");

            var result = query
                .Where(s => s.IsBought == false)
                .Take(pageSize)
                .Select(s => new ShoppingProductDTO
                {
                    Id = s.Id,
                    Content = s.Content,
                    IsBought = s.IsBought,
                    CreatedAt = s.CreatedAt,
                    UserId = s.UserId,
                    UserName = s.User.FullName,
                    UserPicture = s.User.ProfilePictureUrl!
                }).ToList();

            return Result<IList<ShoppingProductDTO>>.Success(result);
        }
        public async Task<Result<ShoppingProductDTO?>> UpdateShopProductAsync(ShoppingProductDTO shopProductDTO)
        {
            var query = await _shoppingProductRepository.GetByIdAsync(shopProductDTO.Id);
            if (query == null) return Result<ShoppingProductDTO?>.Failure("Shopping product not found!");

            query.Content = Utilities.CapitalizeFirstLetter(shopProductDTO.Content);
            query.CreatedAt = shopProductDTO.CreatedAt;
            query.IsBought = shopProductDTO.IsBought;

            var user = await _userRepository.GetByIdAsync(shopProductDTO.UserId);
            query.UserId = shopProductDTO.UserId;
            if (user == null) return Result<ShoppingProductDTO?>.Failure(UserMessages.UserNotFoundById);
            query.User = user;

            await _shoppingProductRepository.UpdateAsync(query);

            var updatedDbProduct = await GetShopProductByIdAsync(query.Id);
            if (updatedDbProduct.Value == null) return Result<ShoppingProductDTO?>.Failure("Error occurred while updating the shopping product!");

            return Result<ShoppingProductDTO?>.Success(updatedDbProduct.Value);
        }
        public async Task<Result<ShoppingProductDTO?>> CreateShopProductAsync(CreateShoppingProductDTO shopProductDTO)
        {
            var user = await _userRepository.GetByIdAsync(shopProductDTO.UserId);
            if(user == null) return Result<ShoppingProductDTO?>.Failure(UserMessages.UserNotFoundById);
            var newShopProduct = new ShoppingProduct
            {
                Id = Guid.NewGuid(),
                Content = Utilities.CapitalizeFirstLetter(shopProductDTO.Content),
                IsBought = shopProductDTO.IsBought,
                CreatedAt = shopProductDTO.CreatedAt,
                UserId = shopProductDTO.UserId,
                User = user
            };
            await _shoppingProductRepository.CreateAsync(newShopProduct);

            var query = await _shoppingProductRepository.GetByIdAsync(newShopProduct.Id);
            if (query == null) return Result<ShoppingProductDTO?>.Failure("Error occurred while creating the shopping product!");

            return Result<ShoppingProductDTO?>.Success(_mapper.Map<ShoppingProductDTO>(query));
        }
        public async Task<Result<IList<ShoppingProductDTO>>> GetAllShopProductsByUserEmailAsync(string email, int pageSize = _defaultPageSize)
        {
            var query = await _shoppingProductRepository.GetAllShopProductsByUserEmailAsync(email);
            if (query == null || !query.Any())
                return Result<IList<ShoppingProductDTO>>.Success(new List<ShoppingProductDTO>());

            var result = query
                 .Take(pageSize)
                .Select(s => new ShoppingProductDTO
                {
                    Id = s.Id,
                    Content = s.Content,
                    IsBought = s.IsBought,
                    CreatedAt = s.CreatedAt,
                    UserId = s.UserId,
                    UserName = s.User?.FullName ?? "Unknown",
                    UserPicture = s.User?.ProfilePictureUrl ?? string.Empty
                }).ToList();

            return Result<IList<ShoppingProductDTO>>.Success(result);
        }
    }
}