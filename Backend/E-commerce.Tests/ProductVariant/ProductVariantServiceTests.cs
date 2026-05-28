using System;
using System.Threading.Tasks;
using E_commerce.DTOs.Variant;
using E_commerce.Models;
using E_commerce.Repositories.Interfaces;
using E_commerce.Services;
using Moq;
using Xunit;

namespace E_commerce.Tests.ProductVariantTests
{
    public class ProductVariantServiceTests
    {
        private readonly Mock<IProductVariantRepository> _variantRepo;
        private readonly Mock<IProductRepository> _productRepo;
        private readonly ProductVariantService _service;

        public ProductVariantServiceTests()
        {
            _variantRepo = new Mock<IProductVariantRepository>();
            _productRepo = new Mock<IProductRepository>();
            _service = new ProductVariantService(_variantRepo.Object, _productRepo.Object);
        }

        [Fact]
        public async Task AddVariant_AddsVariantAndReturnsResponse()
        {
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Sample Product",
                Price = 10m,
                CategoryId = Guid.NewGuid(),
                BrandId = Guid.NewGuid()
            };

            _productRepo.Setup(r => r.GetById(productId)).ReturnsAsync(product);
            _variantRepo.Setup(r => r.Add(It.IsAny<ProductVariant>())).Returns(Task.CompletedTask);
            _variantRepo.Setup(r => r.SaveChanges()).ReturnsAsync(true);

            var request = new CreateVariantRequest
            {
                Name = "Size M",
                Price = 15m,
                Quatity = 2
            };

            var result = await _service.AddVariant(productId, request);

            Assert.NotNull(result);
            Assert.Equal("Size M", result.Name);
            Assert.Equal(15m, result.Price);
            Assert.Equal(2, result.Quatity);

            _variantRepo.Verify(r => r.Add(It.Is<ProductVariant>(v =>
                v.ProductId == productId &&
                v.Name == "Size M" &&
                v.Price == 15m &&
                v.Quantity == 2)), Times.Once);
            _variantRepo.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task UpdateVariant_UpdatesVariantAndReturnsResponse()
        {
            var variantId = Guid.NewGuid();
            var variant = new ProductVariant
            {
                Id = variantId,
                ProductId = Guid.NewGuid(),
                Name = "Size M",
                Price = 15m,
                Quantity = 2
            };

            _variantRepo.Setup(r => r.GetById(variantId)).ReturnsAsync(variant);
            _variantRepo.Setup(r => r.SaveChanges()).ReturnsAsync(true);

            var request = new UpdateVariantRequest
            {
                Name = "Size L",
                Price = 20m,
                Quantity = 3
            };

            var result = await _service.UpdateVariant(variantId, request);

            Assert.Equal("Size L", result.Name);
            Assert.Equal(20m, result.Price);
            Assert.Equal(3, result.Quatity);
            _variantRepo.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task DeleteVariant_RemovesVariant()
        {
            var variantId = Guid.NewGuid();
            var variant = new ProductVariant
            {
                Id = variantId,
                ProductId = Guid.NewGuid(),
                Name = "Size M",
                Price = 15m,
                Quantity = 2
            };

            _variantRepo.Setup(r => r.GetById(variantId)).ReturnsAsync(variant);
            _variantRepo.Setup(r => r.SaveChanges()).ReturnsAsync(true);

            await _service.DeleteVariant(variantId);

            _variantRepo.Verify(r => r.Remove(variant), Times.Once);
            _variantRepo.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task AddVariant_ThrowsKeyNotFoundException_WhenProductNotFound()
        {
            var missingProductId = Guid.NewGuid();
            var request = new CreateVariantRequest
            {
                Name = "Size M",
                Price = 15m,
                Quatity = 2
            };

            _productRepo.Setup(r => r.GetById(missingProductId)).ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AddVariant(missingProductId, request));
        }
    }
}
