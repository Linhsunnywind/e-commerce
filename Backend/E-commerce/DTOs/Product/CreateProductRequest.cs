using E_commerce.Models;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Products
{
    public class CreateProductRequest
    {
        // nhận dữ liệu và tạo sản phẩm
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = null!;
        [StringLength(2000)]
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public Guid BrandId { get; set; }
        public List<CreateProductVariantRequest> Variants { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new();
    }
    public class CreateProductVariantRequest
    {
        [Required(ErrorMessage = "Variant name is required.")]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }
    }

    public class UpdateProductRequest
    {
        [StringLength(200, MinimumLength = 1)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }

        [Range(0, 5, ErrorMessage = "AverageRating must be between 0 and 5.")]
        public double? AverageRating { get; set; }

        // null = không update, empty list = xóa hết
        public List<UpdateProductVariantRequest>? Variants { get; set; }
        public List<string>? ImageUrls { get; set; }
    }

    public class UpdateProductVariantRequest
    {
        public Guid? Id { get; set; } // null = thêm mới, có Id = update
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
