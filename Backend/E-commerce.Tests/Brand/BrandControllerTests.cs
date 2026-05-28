using E_commerce.Controllers.BrandController;
using E_commerce.DTOs.Brand;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace E_commerce.Tests.Brand
{
    public class BrandControllerTests
    {
        private readonly Mock<IBrandService> _service = new();
        private readonly BrandController _controller;

        public BrandControllerTests()
        {
            _controller = new BrandController(_service.Object);
        }

        [Fact]
        public async Task GetBrands_ReturnsOk_WithBrandList()
        {
            _service.Setup(s => s.GetBrands()).ReturnsAsync(new List<BrandResponse>
            {
                new BrandResponse { Id = Guid.NewGuid(), Name = "Apple" },
                new BrandResponse { Id = Guid.NewGuid(), Name = "Samsung" }
            });

            var result = await _controller.GetBrands();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<BrandResponse>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal(2, payload.Data!.Count);
        }

        [Fact]
        public async Task CreateBrand_ReturnsOk_WithCreatedBrand()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.CreateBrand(It.IsAny<BrandRequest>()))
                .ReturnsAsync(new BrandResponse { Id = id, Name = "Sony" });

            var result = await _controller.CreateBrand(new BrandRequest { Name = "Sony" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<BrandResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("Sony", payload.Data!.Name);
        }

        [Fact]
        public async Task UpdateBrand_ReturnsOk_WithUpdatedBrand()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateBrand(id, It.IsAny<BrandRequest>()))
                .ReturnsAsync(new BrandResponse { Id = id, Name = "LG" });

            var result = await _controller.UpdateBrand(id, new BrandRequest { Name = "LG" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<BrandResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("LG", payload.Data!.Name);
        }

        [Fact]
        public async Task DeleteBrand_ReturnsOk_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteBrand(id)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteBrand(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }
    }
}
