using E_commerce.Data;
using E_commerce.Models;
using E_commerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetProductById(Guid productId)
        {
            return await _context.Products
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<Review?> GetReviewById(Guid reviewId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task AddReview(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public void DeleteReview(Review review)
        {
            _context.Reviews.Remove(review);
        }

        public async Task<double> CalculateAverageRating(Guid productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<bool> HasUserPurchasedProduct(Guid userId, Guid productId)
        {
            return await _context.OrderDetails
                .AnyAsync(od =>
                    od.Order.UserId == userId &&
                    od.Order.Status == OrderStatus.Delivered &&
                    od.ProductVariant.ProductId == productId);
        }

        public async Task<Review?> GetUserReviewForProduct(Guid userId, Guid productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }
    }
}