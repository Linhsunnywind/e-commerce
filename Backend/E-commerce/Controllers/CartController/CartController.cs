using E_commerce.DTOs.Cart;
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
                return NotFound("Cart not found");

            return Ok(cart);
        }

        // POST: /api/cart/items
        [HttpPost("add")]
        public async Task<IActionResult> AddItem(AddCartItem dto)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _cartService.AddItemAsync(userId, dto);

            if (!result)
                return BadRequest("Cannot add item");

            return Ok("Add item successfully");
        }

        // PUT: /api/cart/items/{id}
        [HttpPut("update")]
        public async Task<IActionResult> UpdateItem(UpdateCartItem dto)
        {
            var result = await _cartService.UpdateItemAsync(dto);

            if (!result)
                return BadRequest("Cannot update item");

            return Ok("Update item successfully");
        }

        // DELETE: /api/cart/items/{id}
        [HttpDelete("remove")]
        public async Task<IActionResult> DeleteItem(Guid productVariantId)
        {
            var result = await _cartService.DeleteItemAsync(productVariantId);

            if (!result)
                return NotFound("Cart item not found");

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