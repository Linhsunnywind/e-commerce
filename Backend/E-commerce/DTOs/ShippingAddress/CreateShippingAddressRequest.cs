using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.ShippingAddress
{
    public class CreateShippingAddressRequest
    {
        [Required] public string FullName { get; set; } = null!;
        [Required] public string PhoneNumber { get; set; } = null!;
        [Required] public string Province { get; set; } = null!;
        [Required] public string District { get; set; } = null!;
        [Required] public string Ward { get; set; } = null!;
        [Required] public string Street { get; set; } = null!;
        public bool IsDefault { get; set; } = false;
    }
}
