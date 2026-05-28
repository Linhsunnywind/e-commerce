using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Category
{
    public class CategoryRequest
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;
    }
}