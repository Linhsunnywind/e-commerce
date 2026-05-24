using E_commerce.Data;
using E_commerce.DTOs.Category;
using E_commerce.Models;
using E_commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryResponse>> GetCategories()
        {
            return await _context.Categories
                .Where(c => !c.IsDeleted)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryResponse> CreateCategory(CategoryRequest request)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            await _context.Categories.AddAsync(category);

            await _context.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryResponse> UpdateCategory(Guid id, CategoryRequest request)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            category.Name = request.Name;

            await _context.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task DeleteCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            category.IsDeleted = true;

            await _context.SaveChangesAsync();
        }
    }
}