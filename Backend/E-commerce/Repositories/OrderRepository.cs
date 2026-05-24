using E_commerce.Data;
using E_commerce.Models;
using E_commerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAll()
        {
            return await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(o => o.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .Include(o => o.PaymentMethod)
            .Include(o => o.SupportRequest)
            .Include(o => o.Voucher)
            .Include(o => o.User)
            .ToListAsync();
        }

        public async Task<Order?> GetById(Guid id)
        {
            return await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .Include(o => o.PaymentMethod)
            .Include(o => o.SupportRequest)
            .Include(o => o.Voucher)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<List<Order>> GetByUserId(Guid userId)
        {
            return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .Include(o => o.PaymentMethod)
            .Include(o => o.SupportRequest)
            .Include(o => o.Voucher)
            .Include(o => o.User)
            .ToListAsync();
        }
        public async Task AddOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
        }
        public void RemoveDetails(IEnumerable<OrderDetail> details)
        {
            _context.OrderDetails.RemoveRange(details);
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
