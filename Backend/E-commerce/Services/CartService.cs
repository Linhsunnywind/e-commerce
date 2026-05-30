using E_commerce.Data;
using E_commerce.DTOs.Cart;
using E_commerce.Models;
using E_commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace E_commerce.Services.Implementations
{
    // Service xử lý business logic của Cart
    public class CartService : ICartService
    {
        // DbContext dùng để thao tác database
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }
        // Lấy cart của user
        public async Task<CartResponse?> GetCartAsync(Guid userId)
        {
            // Tìm cart theo UserId
            // Include để load CartItems
            // ThenInclude để load ProductVariant của từng CartItem
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // Không tìm thấy cart
            if (cart == null)
                return null;

            // Mapping Entity -> DTO
            return new CartResponse
            {
                Id = cart.Id,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    ProductVariantId = ci.ProductVariantId,
                    ProductName = ci.ProductVariant.Product != null ? ci.ProductVariant.Product.Name : string.Empty,
                    ProductVariantName = ci.ProductVariant.Name,
                    Price = ci.ProductVariant.Price,
                    Quantity = ci.Quantity
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.ProductVariant.Price)
            };
        }
        // Thêm item vào cart
        public async Task<bool> AddItemAsync(Guid userId, AddCartItem dto)
        {
            // Tìm cart của user
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // Nếu chưa có cart thì tạo mới
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Tìm ProductVariant
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.Id == dto.ProductVariantId);

            // Không tìm thấy variant
            if (variant == null)
                return false;

            // Kiểm tra stock
            // Quantity trong ProductVariant đóng vai trò stock
            if (variant.Quantity < dto.Quantity)
                return false;

            // Kiểm tra item đã tồn tại trong cart chưa
            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductVariantId == dto.ProductVariantId);

            // Nếu item đã tồn tại -> cộng thêm quantity
            if (existingItem != null)
            {
                // Kiểm tra tổng quantity có vượt stock không
                if (existingItem.Quantity + dto.Quantity > variant.Quantity) return false;   

                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                // Nếu chưa tồn tại -> tạo mới CartItem
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductVariantId = dto.ProductVariantId,
                    Quantity = dto.Quantity
                };

                _context.CartItems.Add(cartItem);
            }

            // Lưu thay đổi xuống database
            await _context.SaveChangesAsync();

            return true;
        }
        // Update quantity của CartItem
        public async Task<bool> UpdateItemAsync(UpdateCartItem dto)
        {
            // Tìm CartItem theo Id
            var item = await _context.CartItems
                .Include(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(ci => ci.ProductVariantId == dto.ProductVariantId);

            // Không tìm thấy item
            if (item == null)
                return false;

            // Kiểm tra stock
            if (item.ProductVariant.Quantity < dto.Quantity)
                return false;

            // Update quantity
            item.Quantity = dto.Quantity;

            // Save database
            await _context.SaveChangesAsync();

            return true;
        }

        // Xóa CartItem
        public async Task<bool> DeleteItemAsync(Guid productVariantId)
        {
            // Tìm CartItem
            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductVariantId == productVariantId);

            // Không tìm thấy
            if (item == null)
                return false;

            // Xóa item
            _context.CartItems.Remove(item);

            // Save database
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task RemoveItemsByIdsAsync(List<Guid> itemIds)
        {
            var items = await _context.CartItems
                .Where(ci => itemIds.Contains(ci.Id))
                .ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        // Xóa toàn bộ cart
        public async Task<bool> ClearCartAsync(Guid userId)
        {
            // Tìm cart theo userId
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // Không tìm thấy cart
            if (cart == null)
                return false;

            // Xóa tất cả cart items
            _context.CartItems.RemoveRange(cart.CartItems);

            // Save database
            await _context.SaveChangesAsync();

            return true;
        }
    }
}