using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Variant
{
    public class CreateVariantRequest
    {
        [Required(ErrorMessage = "Variant name is required.")]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quatity { get; set; }
    }
}