using E_commerce.Controllers;
using E_commerce.DTOs.SupportRequest;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace E_commerce.Tests.Support
{
    public class SupportControllerTests
    {
        private readonly Mock<ISupportService> _service = new();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly SupportController _controller;

        public SupportControllerTests()
        {
            _controller = new SupportController(_service.Object);
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

        private SupportRequestResponseDto FakeTicket() => new SupportRequestResponseDto
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            CustomerName = "Nguyen Van A",
            Subject = "Sản phẩm bị lỗi",
            Message = "iPhone 16 Pro mua hôm qua không bật được nguồn",
            Status = "Open",
            CreatedDate = DateTime.UtcNow
        };

        [Fact]
        public async Task CreateTicket_ReturnsOk_WhenTicketCreated()
        {
            _service.Setup(s => s.CreateTicketAsync(_userId, It.IsAny<CreateSupportRequestDto>()))
                .ReturnsAsync(FakeTicket());

            var result = await _controller.CreateTicket(new CreateSupportRequestDto
            {
                Subject = "Sản phẩm bị lỗi",
                Message = "iPhone 16 Pro mua hôm qua không bật được nguồn"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<SupportRequestResponseDto>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task GetMyTickets_ReturnsOk_WithTicketList()
        {
            _service.Setup(s => s.GetTicketsByUserIdAsync(_userId))
                .ReturnsAsync(new List<SupportRequestResponseDto> { FakeTicket() });

            var result = await _controller.GetMyTickets();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<IEnumerable<SupportRequestResponseDto>>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsOk_WhenStatusUpdated()
        {
            var ticketId = Guid.NewGuid();
            var ticket = FakeTicket();
            ticket.Status = "Resolved";
            _service.Setup(s => s.UpdateTicketStatusAsync(ticketId, "Resolved")).ReturnsAsync(ticket);

            var result = await _controller.UpdateStatus(ticketId, new UpdateSupportStatusDto { Status = "Resolved" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<SupportRequestResponseDto>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsNotFound_WhenTicketNotFound()
        {
            var ticketId = Guid.NewGuid();
            _service.Setup(s => s.UpdateTicketStatusAsync(ticketId, It.IsAny<string>()))
                .ThrowsAsync(new KeyNotFoundException("Ticket not found"));

            var result = await _controller.UpdateStatus(ticketId, new UpdateSupportStatusDto { Status = "Resolved" });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsBadRequest_WhenStatusInvalid()
        {
            var ticketId = Guid.NewGuid();
            _service.Setup(s => s.UpdateTicketStatusAsync(ticketId, It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException("Invalid status"));

            var result = await _controller.UpdateStatus(ticketId, new UpdateSupportStatusDto { Status = "InvalidStatus" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAllTickets_ReturnsOk_WithFilteredList()
        {
            _service.Setup(s => s.GetAllTicketsAsync("Open"))
                .ReturnsAsync(new List<SupportRequestResponseDto> { FakeTicket() });

            var result = await _controller.GetAllTickets("Open");

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<IEnumerable<SupportRequestResponseDto>>>(ok.Value);
            Assert.True(payload.Success);
        }
    }
}
