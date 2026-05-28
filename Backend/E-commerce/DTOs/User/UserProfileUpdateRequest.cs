using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.User
{
    public class UserProfileUpdateRequest
    {
        [StringLength(100, ErrorMessage = "Name must be at most 100 characters")]
        public string? FullName { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
