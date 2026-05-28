using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Variant
{
    public class UpdateVariantRequest
    {
        [StringLength(200, MinimumLength = 1)]
        public string? Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int? Quantity { get; set; }
    }
}
