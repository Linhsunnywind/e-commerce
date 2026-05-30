using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Cart
{
    public class UpdateCartItem
    {
        public Guid ProductVariantId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}