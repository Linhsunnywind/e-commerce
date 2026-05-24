using System.Threading.Tasks;
using Moq;
using Xunit;
using E-commerce.Models;
using E-commerce.Services;
using E-commerce.Repositories;

namespace E-commerce.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            // Thiết lập Mock cho Repository và tiêm (inject) vào Service
            _mockRepo = new Mock<IProductRepository>();
            _productService = new ProductService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnSavedProduct_WhenSuccessful()
        {
            // Arrange (Chuẩn bị)
            var newProduct = new Product { Id = 1, Name = "Laptop Dell XPS", Price = 1500, Category = "Electronics" };
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Product>())).ReturnsAsync(newProduct);

            // Act (Thực thi)
            var result = await _productService.CreateProductAsync(newProduct);

            // Assert (Xác nhận)
            Assert.NotNull(result);
            Assert.Equal("Laptop Dell XPS", result.Name);
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once); // Đảm bảo hàm Add được gọi 1 lần
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProduct_WhenIdExists()
        {
            // Arrange
            var mockProduct = new Product { Id = 1, Name = "Laptop Dell XPS", Price = 1500 };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mockProduct);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnUpdatedProduct_WhenSuccessful()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, Name = "Laptop Dell XPS", Price = 1500 };
            var updatedInfo = new Product { Id = 1, Name = "Laptop Dell XPS 15", Price = 1600 };
            
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingProduct);
            _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(updatedInfo);

            // Act
            var result = await _productService.UpdateProductAsync(1, updatedInfo);

            // Assert
            Assert.Equal("Laptop Dell XPS 15", result.Name);
            Assert.Equal(1600, result.Price);
            _mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ShouldCallDeleteMethod_Correctly()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, Name = "Laptop Dell XPS" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingProduct);
            _mockRepo.Setup(repo => repo.DeleteAsync(existingProduct)).Returns(Task.CompletedTask);

            // Act
            await _productService.DeleteProductAsync(1);

            // Assert
            _mockRepo.Verify(repo => repo.DeleteAsync(existingProduct), Times.Once); // Khẳng định hàm xóa kích hoạt đúng 1 lần
        }
    }
}