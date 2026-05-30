using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.SupportRequest
{
    public class CreateSupportRequestDto
    {
        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(200, MinimumLength = 1)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(2000, MinimumLength = 1)]
        public string Message { get; set; } = string.Empty;
    }
}