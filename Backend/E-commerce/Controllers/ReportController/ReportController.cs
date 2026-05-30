using E_commerce.DTOs.Report;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/reports")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: /api/report/revenue
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("Invalid date range");

            var result = await _reportService
                .GetRevenueReportAsync(startDate, endDate);

            return Ok(BaseResponse<RevenueReport>.Ok(result));
        }

        // GET: /api/report/orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrderStatistics()
        {
            var result = await _reportService
                .GetOrderStatisticsAsync();

            return Ok(BaseResponse<List<OrderStatistics>>.Ok(result));
        }

        // GET: /api/report/customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetTopCustomers(
            [FromQuery] int top = 5)
        {
            if (top <= 0)
                return BadRequest("Top must be greater than 0");

            var result = await _reportService
                .GetTopCustomersAsync(top);

            return Ok(BaseResponse<List<TopCustomer>>.Ok(result));

        }
    }
}