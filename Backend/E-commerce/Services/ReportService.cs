using E_commerce.Data;
using E_commerce.DTOs.Report;
using E_commerce.Models;
using E_commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        // Revenue Report
        public async Task<RevenueReport> GetRevenueReportAsync(
            DateTime startDate,
            DateTime endDate)
        {
            var deliveredOrders = await _context.Orders
                .Where(o =>
                    o.Status == OrderStatus.Delivered &&
                    o.OrderDate >= startDate &&
                    o.OrderDate <= endDate)
                .ToListAsync();

            return new RevenueReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = deliveredOrders.Sum(o => o.TotalAmount),
                TotalOrders = deliveredOrders.Count
            };
        }

        // Order Statistics
        public async Task<List<OrderStatistics>> GetOrderStatisticsAsync()
        {
            var statusLabels = new Dictionary<OrderStatus, string>
            {
                { OrderStatus.Pending,    "Chờ xử lý" },
                { OrderStatus.Processing, "Đang xử lý" },
                { OrderStatus.Shipped,    "Đang giao" },
                { OrderStatus.Delivered,  "Đã giao" },
                { OrderStatus.Cancelled,  "Đã hủy" },
            };
            var data = await _context.Orders
            .GroupBy(o => o.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count(),
                TotalRevenue = g.Sum(x => x.TotalAmount),
                AverageOrderValue = g.Average(x => x.TotalAmount)
            })
            .ToListAsync();

        return statusLabels.Keys.Select(status =>
            {
                var found = data.FirstOrDefault(d => d.Status == status);
                return new OrderStatistics
                {
                    Status = statusLabels[status],
                    Count = found?.Count ?? 0,
                    TotalRevenue = found?.TotalRevenue ?? 0,
                    AverageOrderValue = found?.AverageOrderValue ?? 0
                };
            }).ToList();

        }

        // Top Customers
        public async Task<List<TopCustomer>> GetTopCustomersAsync(int top)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Where(o => o.Status == OrderStatus.Delivered)
                .GroupBy(o => new
                {
                    o.UserId,
                    o.User.FullName,
                    o.User.Email
                })
                .Select(g => new TopCustomer
                {
                    CustomerId = g.Key.UserId,
                    CustomerName = g.Key.FullName ?? "Unknown",
                    Email = g.Key.Email,
                    TotalSpent = g.Sum(x => x.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(top)
                .ToListAsync();
        }
    }
}