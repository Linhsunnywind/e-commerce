using E_commerce.Data;
using E_commerce.Models;
using E_commerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Product> GetProductsQuery()
        {
            return _context.Products.Where(p => !p.IsDeleted).AsNoTracking();
        }


        public async Task<Product?> GetById(Guid id)
        {
            return await _context.Products
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        // tìm đối tượng theo Id và đánh dấu là thay đổi
        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
        }

        public void DeleteProduct(Product product)
        {
            _context.Remove(product);
        }

        public async Task UpdateVariantAsync(Guid id, string name, decimal price, int quantity)
        {
            await _context.ProductVariants
                .Where(v => v.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(v => v.Name, name)
                    .SetProperty(v => v.Price, price)
                    .SetProperty(v => v.Quantity, quantity));
        }

        public async Task InsertVariantAsync(Guid variantId, Guid productId, string name, decimal price, int quantity)
        {
            await _context.Database.ExecuteSqlAsync(
                $"INSERT INTO ProductVariants (Id, ProductId, Name, Price, Quantity) VALUES ({variantId}, {productId}, {name}, {price}, {quantity})");
        }

        public async Task DeleteVariantsAsync(List<Guid> variantIds)
        {
            if (variantIds.Count == 0) return;
            await _context.ProductVariants
                .Where(v => variantIds.Contains(v.Id))
                .ExecuteDeleteAsync();
        }

        public void RemoveVariants(List<ProductVariant> variants)
        {
            _context.ProductVariants.RemoveRange(variants);
        }

        public void RemoveImages(List<ProductImage> images)
        {
            _context.ProductImages.RemoveRange(images);
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
