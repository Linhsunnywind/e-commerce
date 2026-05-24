using E_commerce.Data;
using E_commerce.DTOs.Brand;
using E_commerce.Models;
using E_commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _context;

        public BrandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BrandResponse>> GetBrands()
        {
            return await _context
                .Brands.Select(b => new BrandResponse { Id = b.Id, Name = b.Name })
                .ToListAsync();
        }

        public async Task<BrandResponse> CreateBrand(BrandRequest request)
        {
            var existedBrand = await _context.Brands.AnyAsync(b =>
                b.Name.ToLower() == request.Name.ToLower()
            );
            if (existedBrand)
                throw new Exception("Brand name already exists");
            var brand = new Brand { Id = Guid.NewGuid(), Name = request.Name };
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();

            return new BrandResponse { Id = brand.Id, Name = brand.Name };
        }

        public async Task<BrandResponse> UpdateBrand(Guid id, BrandRequest request)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
                throw new KeyNotFoundException("Brand not found.");

            brand.Name = request.Name;

            await _context.SaveChangesAsync();

            return new BrandResponse { Id = brand.Id, Name = brand.Name };
        }

        public async Task DeleteBrand(Guid id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
                throw new KeyNotFoundException("Brand not found.");

            _context.Brands.Remove(brand);

            await _context.SaveChangesAsync();
        }
    }
}
