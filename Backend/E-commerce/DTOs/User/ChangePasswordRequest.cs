using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.User
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$",
            ErrorMessage = "New password must contain at least one uppercase letter, one number and one special character.")]
        public string NewPassword { get; set; } = null!;
    }
}
