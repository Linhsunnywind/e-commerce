using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using E_commerce.Controllers;
using E_commerce.DTOs.Variant;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;

namespace E_commerce.Tests.ProductVariantTests
{
    public class ProductVariantControllerTests
    {
        private readonly Mock<IProductVariantService> _mockVariantService;
        private readonly ProductVariantController _controller;

        public ProductVariantControllerTests()
        {
            _mockVariantService = new Mock<IProductVariantService>();
            _controller = new ProductVariantController(_mockVariantService.Object);
        }

        [Fact]
        public async Task AddVariant_ReturnsOk_WithVariantResponse()
        {
            var productId = Guid.NewGuid();
            var variantResponse = new VariantResponse
            {
                Id = Guid.NewGuid(),
                Name = "Size M",
                Price = 99.99m,
                Quatity = 5
            };

            _mockVariantService
                .Setup(s => s.AddVariant(productId, It.IsAny<CreateVariantRequest>()))
                .ReturnsAsync(variantResponse);

            var result = await _controller.AddVariant(productId, new CreateVariantRequest
            {
                Name = "Size M",
                Price = 99.99m,
                Quatity = 5
            });

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var response = Assert.IsType<BaseResponse<VariantResponse>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Size M", response.Data!.Name);
            Assert.Equal(5, response.Data.Quatity);
        }

        [Fact]
        public async Task UpdateVariant_ReturnsOk_WithUpdatedVariant()
        {
            var variantId = Guid.NewGuid();
            var updated = new VariantResponse
            {
                Id = variantId,
                Name = "Size L",
                Price = 109.99m,
                Quatity = 10
            };

            _mockVariantService
                .Setup(s => s.UpdateVariant(variantId, It.IsAny<UpdateVariantRequest>()))
                .ReturnsAsync(updated);

            var result = await _controller.UpdateVariant(variantId, new UpdateVariantRequest
            {
                Name = "Size L",
                Price = 109.99m,
                Quantity = 10
            });

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var response = Assert.IsType<BaseResponse<VariantResponse>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(variantId, response.Data!.Id);
            Assert.Equal("Size L", response.Data.Name);
        }

        [Fact]
        public async Task DeleteVariant_ReturnsOk_WhenVariantDeleted()
        {
            var variantId = Guid.NewGuid();
            _mockVariantService.Setup(s => s.DeleteVariant(variantId)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteVariant(variantId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var response = Assert.IsType<BaseResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Deleted successfully", response.Message);
        }
    }
}
