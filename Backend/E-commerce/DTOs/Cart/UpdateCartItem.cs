namespace E_commerce.DTOs.Cart
{
    // DTO dùng để update số lượng CartItem
    public class UpdateCartItem
    {
        // ProductVariantId của cart item cần update
        public Guid ProductVariantId { get; set; }

        // Quantity mới của cart item
        public int Quantity { get; set; }
    }
}