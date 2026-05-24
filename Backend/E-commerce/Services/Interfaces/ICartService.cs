using E_commerce.DTOs.Cart;

namespace E_commerce.Services.Interfaces
{
    // Interface định nghĩa các chức năng của CartService
    public interface ICartService
    {
        // Lấy cart theo userId
        Task<CartResponse?> GetCartAsync(Guid userId);

        // Thêm item vào cart
        Task<bool> AddItemAsync(Guid userId, AddCartItem dto);

        // Update quantity của cart item
        Task<bool> UpdateItemAsync(UpdateCartItem dto);

        // Xóa cart item
        Task<bool> DeleteItemAsync(Guid productVariantId);

        // Xóa toàn bộ cart
        Task<bool> ClearCartAsync(Guid userId);

        // Xóa các item theo danh sách id
        Task RemoveItemsByIdsAsync(List<Guid> itemIds);
    }
}