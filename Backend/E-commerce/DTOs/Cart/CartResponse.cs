namespace E_commerce.DTOs.Cart
{
    // DTO dùng để trả toàn bộ giỏ hàng
    public class CartResponse
    {
        // Id của Cart
        public Guid Id { get; set; }

        // Danh sách cart items
        public List<CartItemDto> Items { get; set; } = new();

        // Tổng tiền của giỏ hàng 
        public decimal TotalPrice {  get; set; }
    }
}