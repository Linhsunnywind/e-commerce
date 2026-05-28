using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.SupportRequest
{
    public class UpdateSupportStatusDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}
