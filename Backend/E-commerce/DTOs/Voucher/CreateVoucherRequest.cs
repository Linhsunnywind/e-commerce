using E_commerce.Models;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Voucher
{
    public class CreateVoucherRequest
    {
        [Required(ErrorMessage = "Voucher code is required.")]
        [StringLength(50, MinimumLength = 1)]
        public string Code { get; set; } = null!;

        [EnumDataType(typeof(DiscountType), ErrorMessage = "Invalid discount type.")]
        public DiscountType DiscountType { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be greater than 0.")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimum order amount cannot be negative.")]
        public decimal MinOrderAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Max discount amount cannot be negative.")]
        public decimal MaxDiscountAmount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Total quantity must be at least 1.")]
        public int TotalQuantity { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}