namespace E_commerce.DTOs.Cart
{
    // DTO dùng để trả dữ liệu từng item trong cart
    public class CartItemDto
    {
        // Id của CartItem
        public Guid Id { get; set; }

        // Id của ProductVariant
        public Guid ProductVariantId { get; set; }

        // Tên Product
        public string ProductName { get; set; } = string.Empty;

        // Tên ProductVariant
        public string ProductVariantName { get; set; }

        // Giá của ProductVariant
        public decimal Price { get; set; }

        // Số lượng sản phẩm trong cart
        public int Quantity { get; set; }
    }
}