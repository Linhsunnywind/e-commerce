using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Voucher
{
    public class ValidateVoucherRequest
    {
        [Required(ErrorMessage = "Voucher code is required.")]
        [StringLength(50, MinimumLength = 1)]
        public string Code { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Order amount must be greater than 0.")]
        public decimal OrderAmount { get; set; }
    }
}