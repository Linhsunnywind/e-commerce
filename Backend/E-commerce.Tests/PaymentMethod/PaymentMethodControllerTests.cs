using E_commerce.Controllers;
using E_commerce.DTOs.PaymentMethod;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace E_commerce.Tests.PaymentMethod
{
    public class PaymentMethodControllerTests
    {
        private readonly Mock<IPaymentMethodService> _service = new();
        private readonly PaymentMethodsController _controller;

        public PaymentMethodControllerTests()
        {
            _controller = new PaymentMethodsController(_service.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithPaymentMethodList()
        {
            _service.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PaymentMethodResponse>
            {
                new PaymentMethodResponse { Id = Guid.NewGuid(), Name = "Chuyển khoản ngân hàng", IsActive = true },
                new PaymentMethodResponse { Id = Guid.NewGuid(), Name = "Thanh toán khi nhận hàng", IsActive = true }
            });

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<PaymentMethodResponse>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal(2, payload.Data!.Count);
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenCreated()
        {
            _service.Setup(s => s.CreateAsync(It.IsAny<CreatePaymentMethodRequest>())).Returns(Task.CompletedTask);

            var result = await _controller.Create(new CreatePaymentMethodRequest { Name = "Ví điện tử MoMo" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenUpdated()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateAsync(id, It.IsAny<UpdatePaymentMethodRequest>())).Returns(Task.CompletedTask);

            var result = await _controller.Update(id, new UpdatePaymentMethodRequest { Name = "Ví ZaloPay" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenPaymentMethodNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateAsync(id, It.IsAny<UpdatePaymentMethodRequest>()))
                .ThrowsAsync(new KeyNotFoundException("Payment method not found"));

            var result = await _controller.Update(id, new UpdatePaymentMethodRequest { Name = "X" });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenPaymentMethodNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteAsync(id)).ThrowsAsync(new KeyNotFoundException("Payment method not found"));

            var result = await _controller.Delete(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
