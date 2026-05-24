using E_commerce.DTOs.ShippingAddress;

namespace E_commerce.Services.Interfaces
{
    public interface IShippingAddressService
    {
        Task<List<ShippingAddressDto>> GetByUserAsync(Guid userId);
        Task<ShippingAddressDto> CreateAsync(Guid userId, CreateShippingAddressRequest request);
        Task<ShippingAddressDto> UpdateAsync(Guid userId, Guid id, CreateShippingAddressRequest request);
        Task DeleteAsync(Guid userId, Guid id);
        Task SetDefaultAsync(Guid userId, Guid id);
    }
}
