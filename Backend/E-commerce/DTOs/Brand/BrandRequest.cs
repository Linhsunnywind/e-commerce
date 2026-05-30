using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Brand
{
    public class BrandRequest
    {
        [Required(ErrorMessage = "Brand name is required.")]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;
    }
}