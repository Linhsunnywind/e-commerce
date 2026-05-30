using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.ShippingAddress
{
    public class CreateShippingAddressRequest
    {
        [Required] [StringLength(100, MinimumLength = 1)] public string FullName { get; set; } = null!;
        [Required] [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")] public string PhoneNumber { get; set; } = null!;
        [Required] [StringLength(100)] public string Province { get; set; } = null!;
        [Required] [StringLength(100)] public string District { get; set; } = null!;
        [Required] [StringLength(100)] public string Ward { get; set; } = null!;
        [Required] [StringLength(200)] public string Street { get; set; } = null!;
        public bool IsDefault { get; set; } = false;
    }
}
