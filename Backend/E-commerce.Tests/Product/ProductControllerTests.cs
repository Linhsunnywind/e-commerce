using E_commerce.Controllers;
using E_commerce.DTOs.Products;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace E_commerce.Tests.Products
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _service = new();
        private readonly ProductsController _controller;

        public ProductControllerTests()
        {
            _controller = new ProductsController(_service.Object);
        }

        private ProductResponse FakeProduct() => new ProductResponse
        {
            Id = Guid.NewGuid(),
            Name = "iPhone 16 Pro",
            Description = "Điện thoại cao cấp của Apple",
            CategoryName = "Điện thoại",
            BrandName = "Apple",
            AverageRating = 4.8,
            MinPrice = 25990000m,
            Variants = new List<ProductVariantResponse>
            {
                new ProductVariantResponse { Id = Guid.NewGuid(), Name = "256GB Titan Đen", Price = 25990000m, Quantity = 10 }
            }
        };

        [Fact]
        public async Task GetAll_ReturnsOk_WithPagedResult()
        {
            _service.Setup(s => s.GetAll(It.IsAny<ProductFilterRequest>()))
                .ReturnsAsync(new PagedResponse<ProductResponse>(
                    new List<ProductResponse> { FakeProduct() }, 1, 1, 10
                ));

            var result = await _controller.GetAll(new ProductFilterRequest());

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<PagedResponse<ProductResponse>>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenProductFound()
        {
            var product = FakeProduct();
            _service.Setup(s => s.GetById(product.Id)).ReturnsAsync(product);

            var result = await _controller.GetById(product.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<ProductResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("iPhone 16 Pro", payload.Data!.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenProductNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.GetById(id)).ThrowsAsync(new KeyNotFoundException("Product not found"));

            var result = await _controller.GetById(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_Returns201_WhenProductCreated()
        {
            var product = FakeProduct();
            _service.Setup(s => s.CreateProduct(It.IsAny<CreateProductRequest>())).ReturnsAsync(product);

            var result = await _controller.Create(new CreateProductRequest
            {
                Name = "iPhone 16 Pro",
                CategoryId = Guid.NewGuid(),
                BrandId = Guid.NewGuid(),
                Variants = new List<CreateProductVariantRequest>
                {
                    new CreateProductVariantRequest { Name = "256GB Titan Đen", Price = 25990000m, Quantity = 10 }
                }
            });

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenProductUpdated()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateProduct(id, It.IsAny<UpdateProductRequest>())).ReturnsAsync(FakeProduct());

            var result = await _controller.Update(id, new UpdateProductRequest { Name = "iPhone 16 Pro Max" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<ProductResponse>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenProductNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateProduct(id, It.IsAny<UpdateProductRequest>()))
                .ThrowsAsync(new KeyNotFoundException("Product not found"));

            var result = await _controller.Update(id, new UpdateProductRequest { Name = "X" });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenProductDeleted()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteProduct(id)).ReturnsAsync(true);

            var result = await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenProductNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteProduct(id)).ThrowsAsync(new KeyNotFoundException("Product not found"));

            var result = await _controller.Delete(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
