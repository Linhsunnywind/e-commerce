using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using E_commerce.Controllers;
using E_commerce.DTOs.SupportRequest;
using E_commerce.Services;

namespace E_commerce.Tests
{
    public class SupportControllerTests
    {
        private readonly Mock<ISupportService> _mockSupportService;
        private readonly SupportController _controller;

        public SupportControllerTests()
        {
            _mockSupportService = new Mock<ISupportService>();
            _controller = new SupportController(_mockSupportService.Object);
        }

        [Fact]
        public async Task TC01_CreateTicket_ShouldReturn201Created_WithStatusOpen()
        {
            // Arrange: Giả lập dữ liệu đầu vào và kết quả từ Service
            var requestDto = new CreateSupportRequestDto { Title = "Lỗi thanh toán", Content = "Tôi không thể thanh toán." };
            var responseDto = new SupportRequestResponseDto { Id = 101, Title = "Lỗi thanh toán", Content = "Tôi không thể thanh toán.", Status = "Open" };
            
            _mockSupportService.Setup(s => s.CreateTicketAsync(It.IsAny<CreateSupportRequestDto>()))
                               .ReturnsAsync(responseDto);

            // Act: Gọi trực tiếp hàm trong Controller
            var result = await _controller.CreateTicket(requestDto);

            // Assert: Kiểm tra mã phản hồi HTTP 201 và trạng thái mặc định "Open"
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            var returnValue = Assert.IsType<SupportRequestResponseDto>(createdResult.Value);
            Assert.Equal(101, returnValue.Id);
            Assert.Equal("Open", returnValue.Status);
        }

        [Fact]
        public async Task TC02_ResolveTicket_ShouldReturn200Ok_WithStatusResolved()
        {
            // Arrange: Giả lập nhân viên/admin trả lời ticket
            long ticketId = 101;
            var replyDto = new ReplySupportRequestDto { ReplyMessage = "Đã hoàn tiền vào ví của bạn." };
            var resolvedResponseDto = new SupportRequestResponseDto { Id = ticketId, Title = "Lỗi thanh toán", Content = "Tôi không thể thanh toán.", Status = "Resolved" };

            _mockSupportService.Setup(s => s.ResolveTicketAsync(ticketId, It.IsAny<ReplySupportRequestDto>()))
                               .ReturnsAsync(resolvedResponseDto);

            // Act
            var result = await _controller.ResolveTicket(ticketId, replyDto);

            // Assert: Kiểm tra HTTP 200 OK và trạng thái "Resolved"
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnValue = Assert.IsType<SupportRequestResponseDto>(okResult.Value);
            Assert.Equal(ticketId, returnValue.Id);
            Assert.Equal("Resolved", returnValue.Status);
        }
    }
}