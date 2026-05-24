using E_commerce.Data;
using E_commerce.DTOs.ShippingAddress;
using E_commerce.Models;
using E_commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Services
{
    public class ShippingAddressService : IShippingAddressService
    {
        private readonly AppDbContext _context;

        public ShippingAddressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShippingAddressDto>> GetByUserAsync(Guid userId)
        {
            return await _context.ShippingAddresses
                .Where(sa => sa.UserId == userId)
                .OrderByDescending(sa => sa.IsDefault)
                .Select(sa => ToDto(sa))
                .ToListAsync();
        }

        public async Task<ShippingAddressDto> CreateAsync(Guid userId, CreateShippingAddressRequest request)
        {
            // Nếu đặt làm mặc định, bỏ mặc định của địa chỉ cũ
            if (request.IsDefault)
                await ClearDefaultAsync(userId);

            // Nếu đây là địa chỉ đầu tiên, tự động đặt mặc định
            bool isFirst = !await _context.ShippingAddresses.AnyAsync(sa => sa.UserId == userId);

            var address = new ShippingAddress
            {
                UserId    = userId,
                FullName  = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Province  = request.Province,
                District  = request.District,
                Ward      = request.Ward,
                Street    = request.Street,
                IsDefault = request.IsDefault || isFirst
            };

            _context.ShippingAddresses.Add(address);
            await _context.SaveChangesAsync();
            return ToDto(address);
        }

        public async Task<ShippingAddressDto> UpdateAsync(Guid userId, Guid id, CreateShippingAddressRequest request)
        {
            var address = await _context.ShippingAddresses
                .FirstOrDefaultAsync(sa => sa.Id == id && sa.UserId == userId)
                ?? throw new KeyNotFoundException("Address not found.");

            if (request.IsDefault && !address.IsDefault)
                await ClearDefaultAsync(userId);

            address.FullName    = request.FullName;
            address.PhoneNumber = request.PhoneNumber;
            address.Province    = request.Province;
            address.District    = request.District;
            address.Ward        = request.Ward;
            address.Street      = request.Street;
            address.IsDefault   = request.IsDefault;

            await _context.SaveChangesAsync();
            return ToDto(address);
        }

        public async Task DeleteAsync(Guid userId, Guid id)
        {
            var address = await _context.ShippingAddresses
                .FirstOrDefaultAsync(sa => sa.Id == id && sa.UserId == userId)
                ?? throw new KeyNotFoundException("Address not found.");

            _context.ShippingAddresses.Remove(address);
            await _context.SaveChangesAsync();

            // Nếu vừa xóa địa chỉ mặc định, tự động set địa chỉ đầu tiên còn lại làm mặc định
            if (address.IsDefault)
            {
                var next = await _context.ShippingAddresses.FirstOrDefaultAsync(sa => sa.UserId == userId);
                if (next != null) { next.IsDefault = true; await _context.SaveChangesAsync(); }
            }
        }

        public async Task SetDefaultAsync(Guid userId, Guid id)
        {
            await ClearDefaultAsync(userId);
            var address = await _context.ShippingAddresses
                .FirstOrDefaultAsync(sa => sa.Id == id && sa.UserId == userId)
                ?? throw new KeyNotFoundException("Address not found.");
            address.IsDefault = true;
            await _context.SaveChangesAsync();
        }

        private async Task ClearDefaultAsync(Guid userId)
        {
            var current = await _context.ShippingAddresses
                .Where(sa => sa.UserId == userId && sa.IsDefault)
                .ToListAsync();
            current.ForEach(sa => sa.IsDefault = false);
            await _context.SaveChangesAsync();
        }

        private static ShippingAddressDto ToDto(ShippingAddress sa) => new()
        {
            Id          = sa.Id,
            FullName    = sa.FullName,
            PhoneNumber = sa.PhoneNumber,
            Province    = sa.Province,
            District    = sa.District,
            Ward        = sa.Ward,
            Street      = sa.Street,
            IsDefault   = sa.IsDefault
        };
    }
}
