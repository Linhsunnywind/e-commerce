using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Name can't be empty")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email can't be empty")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number can't be empty")]
        [StringLength(10, ErrorMessage = "Phone number must be exactly 10 digits")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password has at least 6 characters")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$", 
            ErrorMessage = "Password must contain at least one uppercase letter, one number and one special character")]
        public string Password { get; set; } = string.Empty;
    }
}
