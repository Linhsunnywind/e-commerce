using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.PaymentMethod
{
    public class UpdatePaymentMethodRequest
    {
        [Required(ErrorMessage = "Payment method name is required.")]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}