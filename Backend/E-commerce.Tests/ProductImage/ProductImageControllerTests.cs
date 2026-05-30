using E_commerce.Controllers;
using E_commerce.DTOs.Image;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace E_commerce.Tests.ProductImage
{
    public class ProductImageControllerTests
    {
        private readonly Mock<IProductImageService> _service = new();
        private readonly ProductImageController _controller;

        public ProductImageControllerTests()
        {
            var cloudinary = new CloudinaryDotNet.Cloudinary("cloudinary://key:secret@cloud");
            _controller = new ProductImageController(_service.Object, cloudinary);
        }

        [Fact]
        public async Task AddImage_ReturnsOk_WhenImageAdded()
        {
            var productId = Guid.NewGuid();
            _service.Setup(s => s.AddImageAsync(productId, It.IsAny<AddImageRequest>())).Returns(Task.CompletedTask);

            var result = await _controller.AddImage(productId, new AddImageRequest { Url = "https://res.cloudinary.com/demo/image/upload/sample.jpg" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task AddImage_ReturnsNotFound_WhenProductNotFound()
        {
            var productId = Guid.NewGuid();
            _service.Setup(s => s.AddImageAsync(productId, It.IsAny<AddImageRequest>()))
                .ThrowsAsync(new KeyNotFoundException("Product not found"));

            var result = await _controller.AddImage(productId, new AddImageRequest { Url = "https://res.cloudinary.com/demo/image/upload/sample.jpg" });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteImage_ReturnsOk_WhenImageDeleted()
        {
            var imageId = Guid.NewGuid();
            _service.Setup(s => s.DeleteImageAsync(imageId)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteImage(imageId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task DeleteImage_ReturnsNotFound_WhenImageNotFound()
        {
            var imageId = Guid.NewGuid();
            _service.Setup(s => s.DeleteImageAsync(imageId)).ThrowsAsync(new KeyNotFoundException("Image not found"));

            var result = await _controller.DeleteImage(imageId);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
