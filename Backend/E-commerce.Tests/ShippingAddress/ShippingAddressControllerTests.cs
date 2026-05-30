using E_commerce.Controllers;
using E_commerce.DTOs.ShippingAddress;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace E_commerce.Tests.ShippingAddress
{
    public class ShippingAddressControllerTests
    {
        private readonly Mock<IShippingAddressService> _service = new();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly ShippingAddressController _controller;

        public ShippingAddressControllerTests()
        {
            _controller = new ShippingAddressController(_service.Object);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString()),
                new Claim(ClaimTypes.Role, "Customer")
            }, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        private ShippingAddressDto FakeAddress() => new ShippingAddressDto
        {
            Id = Guid.NewGuid(),
            FullName = "Nguyen Van A",
            PhoneNumber = "0901234567",
            Province = "TP. Hồ Chí Minh",
            District = "Quận 1",
            Ward = "Phường Bến Nghé",
            Street = "123 Nguyễn Huệ",
            IsDefault = true
        };

        [Fact]
        public async Task GetAll_ReturnsOk_WithAddressList()
        {
            _service.Setup(s => s.GetByUserAsync(_userId))
                .ReturnsAsync(new List<ShippingAddressDto> { FakeAddress() });

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<ShippingAddressDto>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Single(payload.Data!);
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenAddressCreated()
        {
            _service.Setup(s => s.CreateAsync(_userId, It.IsAny<CreateShippingAddressRequest>()))
                .ReturnsAsync(FakeAddress());

            var result = await _controller.Create(new CreateShippingAddressRequest
            {
                FullName = "Nguyen Van A",
                PhoneNumber = "0901234567",
                Province = "TP. Hồ Chí Minh",
                District = "Quận 1",
                Ward = "Phường Bến Nghé",
                Street = "123 Nguyễn Huệ"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<ShippingAddressDto>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenAddressUpdated()
        {
            var id = Guid.NewGuid();
            var updated = FakeAddress();
            updated.Street = "456 Lê Lợi";
            _service.Setup(s => s.UpdateAsync(_userId, id, It.IsAny<CreateShippingAddressRequest>()))
                .ReturnsAsync(updated);

            var result = await _controller.Update(id, new CreateShippingAddressRequest
            {
                FullName = "Nguyen Van A",
                PhoneNumber = "0901234567",
                Province = "TP. Hồ Chí Minh",
                District = "Quận 1",
                Ward = "Phường Bến Thành",
                Street = "456 Lê Lợi"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<ShippingAddressDto>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenAddressNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateAsync(_userId, id, It.IsAny<CreateShippingAddressRequest>()))
                .ThrowsAsync(new KeyNotFoundException("Address not found"));

            var result = await _controller.Update(id, new CreateShippingAddressRequest
            {
                FullName = "X", PhoneNumber = "0901234567",
                Province = "X", District = "X", Ward = "X", Street = "X"
            });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteAsync(_userId, id)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenAddressNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteAsync(_userId, id)).ThrowsAsync(new KeyNotFoundException("Address not found"));

            var result = await _controller.Delete(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SetDefault_ReturnsOk_WhenDefaultSet()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.SetDefaultAsync(_userId, id)).Returns(Task.CompletedTask);

            var result = await _controller.SetDefault(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task SetDefault_ReturnsNotFound_WhenAddressNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.SetDefaultAsync(_userId, id)).ThrowsAsync(new KeyNotFoundException("Address not found"));

            var result = await _controller.SetDefault(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
