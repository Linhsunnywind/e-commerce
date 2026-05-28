using E_commerce.Controllers;
using E_commerce.DTOs.Voucher;
using E_commerce.Helpers;
using E_commerce.Models;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace E_commerce.Tests.Voucher
{
    public class VoucherControllerTests
    {
        private readonly Mock<IVoucherService> _service = new();
        private readonly VoucherController _controller;

        public VoucherControllerTests()
        {
            _controller = new VoucherController(_service.Object);
        }

        private VoucherResponse FakeVoucher() => new VoucherResponse
        {
            Id = Guid.NewGuid(),
            Code = "TECH10",
            DiscountType = DiscountType.Percentage,
            DiscountValue = 10m,
            MinOrderAmount = 5000000m,
            MaxDiscountAmount = 500000m,
            TotalQuantity = 100,
            UsedCount = 0,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30),
            IsActive = true
        };

        [Fact]
        public async Task GetAll_ReturnsOk_WithVoucherList()
        {
            _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<VoucherResponse> { FakeVoucher() });

            var result = await _controller.GetAllAsync();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<VoucherResponse>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Single(payload.Data!);
        }

        [Fact]
        public async Task GetVoucherById_ReturnsOk_WhenFound()
        {
            var voucher = FakeVoucher();
            _service.Setup(s => s.GetVoucherById(voucher.Id)).ReturnsAsync(voucher);

            var result = await _controller.GetVoucherById(voucher.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<VoucherResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("TECH10", payload.Data!.Code);
        }

        [Fact]
        public async Task GetVoucherById_ReturnsNotFound_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.GetVoucherById(id)).ThrowsAsync(new KeyNotFoundException("Voucher not found"));

            var result = await _controller.GetVoucherById(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Validate_ReturnsOk_WhenVoucherValid()
        {
            _service.Setup(s => s.Validate(It.IsAny<ValidateVoucherRequest>()))
                .ReturnsAsync(new ValidateVoucherResponse { Code = "TECH10", DiscountAmount = 100000m, FinalAmount = 9900000m });

            var result = await _controller.Validate(new ValidateVoucherRequest { Code = "TECH10", OrderAmount = 10000000m });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<ValidateVoucherResponse>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task Validate_ReturnsNotFound_WhenVoucherNotFound()
        {
            _service.Setup(s => s.Validate(It.IsAny<ValidateVoucherRequest>()))
                .ThrowsAsync(new KeyNotFoundException("Voucher not found"));

            var result = await _controller.Validate(new ValidateVoucherRequest { Code = "INVALID", OrderAmount = 10000000m });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Validate_ReturnsBadRequest_WhenVoucherExpired()
        {
            _service.Setup(s => s.Validate(It.IsAny<ValidateVoucherRequest>()))
                .ThrowsAsync(new InvalidOperationException("Voucher expired"));

            var result = await _controller.Validate(new ValidateVoucherRequest { Code = "EXPIRED", OrderAmount = 10000000m });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateVoucher_ReturnsOk_WhenCreated()
        {
            _service.Setup(s => s.CreateVoucherAsync(It.IsAny<CreateVoucherRequest>())).ReturnsAsync(FakeVoucher());

            var result = await _controller.CreateVoucherAsync(new CreateVoucherRequest
            {
                Code = "TECH10",
                DiscountType = DiscountType.Percentage,
                DiscountValue = 10m,
                MinOrderAmount = 5000000m,
                MaxDiscountAmount = 500000m,
                TotalQuantity = 100,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<VoucherResponse>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task UpdateVoucher_ReturnsOk_WhenUpdated()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateVoucherAsync(id, It.IsAny<UpdateVoucherRequest>())).ReturnsAsync(FakeVoucher());

            var result = await _controller.UpdateVoucherAsync(id, new UpdateVoucherRequest { DiscountValue = 15m });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<VoucherResponse>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task UpdateVoucher_ReturnsNotFound_WhenVoucherNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateVoucherAsync(id, It.IsAny<UpdateVoucherRequest>()))
                .ThrowsAsync(new KeyNotFoundException("Voucher not found"));

            var result = await _controller.UpdateVoucherAsync(id, new UpdateVoucherRequest { DiscountValue = 15m });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteVoucher_ReturnsOk_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task DeleteVoucher_ReturnsNotFound_WhenVoucherNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteAsync(id)).ThrowsAsync(new KeyNotFoundException("Voucher not found"));

            var result = await _controller.DeleteAsync(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
