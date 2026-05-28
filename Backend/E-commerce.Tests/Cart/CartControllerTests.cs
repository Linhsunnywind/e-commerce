using E_commerce.Controllers;
using E_commerce.DTOs.Cart;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace E_commerce.Tests.Cart
{
    public class CartControllerTests
    {
        private readonly Mock<ICartService> _service = new();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _controller = new CartController(_service.Object);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
            }, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Fact]
        public async Task GetCart_ReturnsOk_WhenCartExists()
        {
            var cart = new CartResponse
            {
                Id = Guid.NewGuid(),
                Items = new List<CartItemDto>
                {
                    new CartItemDto { Id = Guid.NewGuid(), ProductName = "iPhone 16 Pro", Quantity = 1, Price = 25990000m }
                },
                TotalPrice = 25990000m
            };
            _service.Setup(s => s.GetCartAsync(_userId)).ReturnsAsync(cart);

            var result = await _controller.GetCart();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<CartResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal(1, payload.Data!.Items.Count);
        }

        [Fact]
        public async Task GetCart_ReturnsNotFound_WhenCartDoesNotExist()
        {
            _service.Setup(s => s.GetCartAsync(_userId)).ReturnsAsync((CartResponse?)null);

            var result = await _controller.GetCart();

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddItem_ReturnsOk_WhenItemAdded()
        {
            _service.Setup(s => s.AddItemAsync(_userId, It.IsAny<AddCartItem>())).ReturnsAsync(true);

            var result = await _controller.AddItem(new AddCartItem { ProductVariantId = Guid.NewGuid(), Quantity = 1 });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task AddItem_ReturnsBadRequest_WhenItemCannotBeAdded()
        {
            _service.Setup(s => s.AddItemAsync(_userId, It.IsAny<AddCartItem>())).ReturnsAsync(false);

            var result = await _controller.AddItem(new AddCartItem { ProductVariantId = Guid.NewGuid(), Quantity = 1 });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateItem_ReturnsOk_WhenUpdated()
        {
            _service.Setup(s => s.UpdateItemAsync(It.IsAny<UpdateCartItem>())).ReturnsAsync(true);

            var result = await _controller.UpdateItem(new UpdateCartItem { ProductVariantId = Guid.NewGuid(), Quantity = 2 });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task UpdateItem_ReturnsBadRequest_WhenUpdateFails()
        {
            _service.Setup(s => s.UpdateItemAsync(It.IsAny<UpdateCartItem>())).ReturnsAsync(false);

            var result = await _controller.UpdateItem(new UpdateCartItem { ProductVariantId = Guid.NewGuid(), Quantity = 2 });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteItem_ReturnsNoContent_WhenDeleted()
        {
            _service.Setup(s => s.DeleteItemAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _controller.DeleteItem(Guid.NewGuid());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteItem_ReturnsNotFound_WhenItemNotFound()
        {
            _service.Setup(s => s.DeleteItemAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _controller.DeleteItem(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ClearCart_ReturnsNoContent_WhenCleared()
        {
            _service.Setup(s => s.ClearCartAsync(_userId)).ReturnsAsync(true);

            var result = await _controller.ClearCart();

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ClearCart_ReturnsNotFound_WhenCartNotFound()
        {
            _service.Setup(s => s.ClearCartAsync(_userId)).ReturnsAsync(false);

            var result = await _controller.ClearCart();

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
