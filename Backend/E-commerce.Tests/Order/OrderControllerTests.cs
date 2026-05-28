using E_commerce.Controllers;
using E_commerce.DTOs.Order;
using E_commerce.Helpers;
using E_commerce.Models;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace E_commerce.Tests.Order
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _service = new();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _controller = new OrderController(_service.Object);
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

        private OrderResponse FakeOrder() => new OrderResponse
        {
            Id = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            TotalAmount = 25990000m,
            Status = OrderStatus.Pending,
            UserId = _userId,
            ReceiverName = "Nguyen Van A",
            ReceiverPhone = "0901234567",
            ShippingAddress = "123 Nguyen Hue, Q1, TP.HCM",
            PaymentMethodName = "Chuyển khoản"
        };

        [Fact]
        public async Task CreateOrder_ReturnsOk_WhenOrderCreated()
        {
            _service.Setup(s => s.CreateOrder(_userId, It.IsAny<CreateOrderRequest>()))
                .ReturnsAsync(FakeOrder());

            var result = await _controller.CreateOrder(new CreateOrderRequest
            {
                ShippingAddressId = Guid.NewGuid(),
                PaymentMethodId = Guid.NewGuid()
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<OrderResponse>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task CreateOrder_ReturnsNotFound_WhenCartItemNotFound()
        {
            _service.Setup(s => s.CreateOrder(_userId, It.IsAny<CreateOrderRequest>()))
                .ThrowsAsync(new KeyNotFoundException("Cart item not found"));

            var result = await _controller.CreateOrder(new CreateOrderRequest
            {
                ShippingAddressId = Guid.NewGuid(),
                PaymentMethodId = Guid.NewGuid()
            });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenOutOfStock()
        {
            _service.Setup(s => s.CreateOrder(_userId, It.IsAny<CreateOrderRequest>()))
                .ThrowsAsync(new InvalidOperationException("Insufficient stock"));

            var result = await _controller.CreateOrder(new CreateOrderRequest
            {
                ShippingAddressId = Guid.NewGuid(),
                PaymentMethodId = Guid.NewGuid()
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithOrderList()
        {
            _service.Setup(s => s.GetAllOrders()).ReturnsAsync(new List<OrderResponse> { FakeOrder() });

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<OrderResponse>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Single(payload.Data!);
        }

        [Fact]
        public async Task GetMyOrders_ReturnsOk_WithUserOrders()
        {
            _service.Setup(s => s.GetMyOrders(_userId)).ReturnsAsync(new List<OrderResponse> { FakeOrder() });

            var result = await _controller.GetMyOrders();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<OrderResponse>>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenOrderFound()
        {
            var order = FakeOrder();
            _service.Setup(s => s.GetOrderById(order.Id)).ReturnsAsync(order);

            var result = await _controller.GetById(order.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<OrderResponse>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenOrderNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.GetOrderById(id)).ThrowsAsync(new KeyNotFoundException("Order not found"));

            var result = await _controller.GetById(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsOk_WhenStatusUpdated()
        {
            var id = Guid.NewGuid();
            var order = FakeOrder();
            order.Status = OrderStatus.Processing;
            _service.Setup(s => s.UpdateStatus(id, It.IsAny<UpdateOrderStatusRequest>())).ReturnsAsync(order);

            var result = await _controller.UpdateStatus(id, new UpdateOrderStatusRequest { OrderStatus = OrderStatus.Processing });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<OrderResponse>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsNotFound_WhenOrderNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateStatus(id, It.IsAny<UpdateOrderStatusRequest>()))
                .ThrowsAsync(new KeyNotFoundException("Order not found"));

            var result = await _controller.UpdateStatus(id, new UpdateOrderStatusRequest { OrderStatus = OrderStatus.Processing });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CancelOrder_ReturnsOk_WhenCancelled()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.CancelOrder(id, _userId)).Returns(Task.CompletedTask);

            var result = await _controller.CancelOrder(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task CancelOrder_ReturnsNotFound_WhenOrderNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.CancelOrder(id, _userId))
                .ThrowsAsync(new KeyNotFoundException("Order not found"));

            var result = await _controller.CancelOrder(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CancelOrder_ReturnsBadRequest_WhenOrderCannotBeCancelled()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.CancelOrder(id, _userId))
                .ThrowsAsync(new InvalidOperationException("Cannot cancel delivered order"));

            var result = await _controller.CancelOrder(id);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
