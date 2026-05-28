using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Staff
{
    public class CreateStaff
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one number and one special character.")]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? FullName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }

    public class UpdateStaff
    {
        public string Name { get; set; }
        public string? FullName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
