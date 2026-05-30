using E_commerce.Controllers;
using E_commerce.DTOs.Report;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace E_commerce.Tests.Report
{
    public class ReportControllerTests
    {
        private readonly Mock<IReportService> _service = new();
        private readonly ReportController _controller;

        public ReportControllerTests()
        {
            _controller = new ReportController(_service.Object);
        }

        [Fact]
        public async Task GetRevenueReport_ReturnsOk_WhenDateRangeValid()
        {
            var start = new DateTime(2026, 1, 1);
            var end = new DateTime(2026, 5, 31);
            _service.Setup(s => s.GetRevenueReportAsync(start, end))
                .ReturnsAsync(new RevenueReport
                {
                    StartDate = start,
                    EndDate = end,
                    TotalRevenue = 500000000m,
                    TotalOrders = 120
                });

            var result = await _controller.GetRevenueReport(start, end);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<RevenueReport>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal(120, payload.Data!.TotalOrders);
        }

        [Fact]
        public async Task GetRevenueReport_ReturnsBadRequest_WhenStartDateAfterEndDate()
        {
            var start = new DateTime(2026, 6, 1);
            var end = new DateTime(2026, 1, 1);

            var result = await _controller.GetRevenueReport(start, end);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetOrderStatistics_ReturnsOk_WithStatistics()
        {
            _service.Setup(s => s.GetOrderStatisticsAsync())
                .ReturnsAsync(new List<OrderStatistics>
                {
                    new OrderStatistics { Status = "Pending", Count = 30, TotalRevenue = 100000000m },
                    new OrderStatistics { Status = "Delivered", Count = 45, TotalRevenue = 150000000m }
                });

            var result = await _controller.GetOrderStatistics();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<OrderStatistics>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal(2, payload.Data!.Count);
        }

        [Fact]
        public async Task GetTopCustomers_ReturnsOk_WithTopList()
        {
            _service.Setup(s => s.GetTopCustomersAsync(5))
                .ReturnsAsync(new List<TopCustomer>
                {
                    new TopCustomer { CustomerId = Guid.NewGuid(), CustomerName = "Nguyen Van A", TotalSpent = 50000000m, OrderCount = 10 }
                });

            var result = await _controller.GetTopCustomers(5);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<TopCustomer>>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task GetTopCustomers_ReturnsBadRequest_WhenTopIsZeroOrNegative()
        {
            var result = await _controller.GetTopCustomers(0);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
