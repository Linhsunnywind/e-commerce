using E_commerce.Controllers.CategoryController;
using E_commerce.DTOs.Category;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace E_commerce.Tests.Category
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _service = new();
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _controller = new CategoryController(_service.Object);
        }

        [Fact]
        public async Task GetCategories_ReturnsOk_WithCategoryList()
        {
            _service.Setup(s => s.GetCategories()).ReturnsAsync(new List<CategoryResponse>
            {
                new CategoryResponse { Id = Guid.NewGuid(), Name = "Điện thoại" },
                new CategoryResponse { Id = Guid.NewGuid(), Name = "Laptop" }
            });

            var result = await _controller.GetCategories();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<CategoryResponse>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal(2, payload.Data!.Count);
        }

        [Fact]
        public async Task CreateCategory_ReturnsOk_WithCreatedCategory()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.CreateCategory(It.IsAny<CategoryRequest>()))
                .ReturnsAsync(new CategoryResponse { Id = id, Name = "Tai nghe" });

            var result = await _controller.CreateCategory(new CategoryRequest { Name = "Tai nghe" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<CategoryResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("Tai nghe", payload.Data!.Name);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsOk_WithUpdatedCategory()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateCategory(id, It.IsAny<CategoryRequest>()))
                .ReturnsAsync(new CategoryResponse { Id = id, Name = "Phụ kiện" });

            var result = await _controller.UpdateCategory(id, new CategoryRequest { Name = "Phụ kiện" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<CategoryResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("Phụ kiện", payload.Data!.Name);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsOk_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteCategory(id)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteCategory(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }
    }
}
