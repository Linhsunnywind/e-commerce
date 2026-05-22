using System.ComponentModel.DataAnnotations;
namespace E_commerce.DTOs.Cart
{
    // DTO dùng để nhận dữ liệu khi user thêm sản phẩm vào giỏ hàng
    public class AddCartItem
    {
        // Id của ProductVariant cần thêm vào cart
        public Guid ProductVariantId { get; set; }

        // Số lượng sản phẩm muốn thêm
        [Range(1,int.MaxValue)]
        public int Quantity { get; set; }
    }
}