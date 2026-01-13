using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ChoresHub.Application.DTOs;
using ChoresHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ChoresHub.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/shopping")]
    [Authorize]
    public class ShoppingProductController(IShoppingProductService shoppingProductService) : ControllerBase
    {
        private readonly IShoppingProductService _shoppingProductService = shoppingProductService;

        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingProductDTO>> GetShopProduct([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID.");

            var result = await _shoppingProductService.GetShopProductByIdAsync(id);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }
        [HttpGet()]
        public async Task<ActionResult<IList<ShoppingProductDTO>>> GetAllShopProducts([FromQuery] int pageSize)
        {
            var result = await _shoppingProductService.GetAllShopProductsAsync(pageSize);

            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Value);
        }
        [HttpGet("all-by-email")]
        public async Task<ActionResult<IList<ShoppingProductDTO>>> GetAllShopProductsByUserEmail([FromQuery] int pageSize)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized("User email not found in cookies.");

            var result = await _shoppingProductService.GetAllShopProductsByUserEmailAsync(userEmail, pageSize);

            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShopProduct([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID.");

            var result = await _shoppingProductService.DeleteShopProductAsync(id);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }
        [HttpPost()]
        public async Task<ActionResult<ShoppingProductDTO>> CreateShopProduct([FromBody] CreateShoppingProductDTO shopProductDTO)
        {
            if (shopProductDTO == null) return BadRequest("Product can not be empty!");

            var result = await _shoppingProductService.CreateShopProductAsync(shopProductDTO);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }
        [HttpPut("update")]
        public async Task<ActionResult<ShoppingProductDTO>> UpdateShopProduct([FromBody] ShoppingProductDTO shopProductDTO)
        {
            if (shopProductDTO == null) return BadRequest("Product can not be empty!");

            var result = await _shoppingProductService.UpdateShopProductAsync(shopProductDTO);
            if (!result.IsSuccess) return NotFound(result.Error);

            return Ok(result.Value);
        }

    }
}