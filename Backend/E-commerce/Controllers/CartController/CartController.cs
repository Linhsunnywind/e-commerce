using E_commerce.DTOs.Cart;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var cart = await _cartService.GetCartAsync(userId);

            if (cart == null)
                return NotFound(BaseResponse<CartResponse>.Fail("Cart not found", 404));

            return Ok(BaseResponse<CartResponse>.Ok(cart));
        }

        // POST: /api/cart/items
        [HttpPost("items")]
        public async Task<IActionResult> AddItem(AddCartItem dto)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _cartService.AddItemAsync(userId, dto);

            if (!result)
                return BadRequest(BaseResponse<string>.Fail("Cannot add item"));

            return Ok(BaseResponse<string>.Ok(null, "Add item successfully"));
        }

        // PUT: /api/cart/items/{id}
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(
            Guid id,
            UpdateCartItem dto
        )
        {
            var result = await _cartService.UpdateItemAsync(id, dto);

            if (!result)
                return BadRequest(BaseResponse<string>.Fail("Cannot update item"));
            return Ok(BaseResponse<string>.Ok(null, "Update item successfully"));
        }

        // DELETE: /api/cart/items/{id}
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var result = await _cartService.DeleteItemAsync(id);

            if (!result)
                return NotFound(BaseResponse<string>.Fail("Cart item not found", 404));

            return NoContent();
        }

        // DELETE: /api/cart
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _cartService.ClearCartAsync(userId);

            if (!result)
                return NotFound("Cart not found");

            return NoContent();
        }
    }
}