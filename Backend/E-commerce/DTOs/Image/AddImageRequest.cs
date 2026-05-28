using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Image
{
    public class AddImageRequest
    {
        [Required(ErrorMessage = "Image URL is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        [StringLength(2048)]
        public string Url { get; set; } = null!;
    }
}