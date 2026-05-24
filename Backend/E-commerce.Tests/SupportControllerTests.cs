using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using E_commerce.Data;
using E_commerce.DTOs.Variant;
using E_commerce.Models;
using E_commerce.Services;

namespace E_commerce.Tests
{
    public class ProductVariantServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ProductVariantService _service;

        public ProductVariantServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new ProductVariantService(_context);
        }

        [Fact]
        public async Task AddVariant_AddsVariantAndReturnsResponse()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Sample Product",
                Price = 10m,
                CategoryId = Guid.NewGuid(),
                BrandId = Guid.NewGuid()
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var request = new CreateVariantRequest
            {
                Name = "Size M",
                Price = 15m,
                Quatity = 2
            };

            var result = await _service.AddVariant(product.Id, request);

            Assert.NotNull(result);
            Assert.Equal("Size M", result.Name);
            Assert.Equal(15m, result.Price);
            Assert.Equal(2, result.Quatity);

            var saved = await _context.ProductVariants.FindAsync(result.Id);
            Assert.NotNull(saved);
            Assert.Equal(product.Id, saved!.ProductId);
        }

        [Fact]
        public async Task UpdateVariant_UpdatesVariantAndReturnsResponse()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Sample Product",
                Price = 10m,
                CategoryId = Guid.NewGuid(),
                BrandId = Guid.NewGuid()
            };
            _context.Products.Add(product);

            var variant = new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Name = "Size M",
                Price = 15m,
                Quantity = 2
            };
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            var request = new UpdateVariantRequest
            {
                Name = "Size L",
                Price = 20m,
                Quantity = 3
            };

            var result = await _service.UpdateVariant(variant.Id, request);

            Assert.Equal("Size L", result.Name);
            Assert.Equal(20m, result.Price);
            Assert.Equal(3, result.Quatity);
        }

        [Fact]
        public async Task DeleteVariant_RemovesVariant()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Sample Product",
                Price = 10m,
                CategoryId = Guid.NewGuid(),
                BrandId = Guid.NewGuid()
            };
            _context.Products.Add(product);

            var variant = new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Name = "Size M",
                Price = 15m,
                Quantity = 2
            };
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            await _service.DeleteVariant(variant.Id);

            var deleted = await _context.ProductVariants.FindAsync(variant.Id);
            Assert.Null(deleted);
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

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AddVariant(missingProductId, request));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
